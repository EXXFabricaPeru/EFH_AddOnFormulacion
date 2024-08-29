CREATE FUNCTION "EXP_FormLM_EXTRUSION"
(
	IN CodFormula nvarchar(50),
	IN ItemCode nvarchar(50),	
	IN TipoFormula nvarchar(2),
	IN Cantidad double
)
-- =============================================
-- Autor           : Quevedo Grimaldo, Ricardo Francisco
-- Fecha Creación  : 15/03/2021
-- =============================================
RETURNS TABLE 
(
	"TIPO" int,
	"RUTA" int,
	"ITEMCODE" nvarchar(50),
	"DESC" nvarchar(100),
	"ORD" int,
	"CANT" double,
	"UNI" nvarchar(2)
)
LANGUAGE SQLSCRIPT
AS

BEGIN
DECLARE formTemp TABLE (RUTA int, TIPO nvarchar(1), TOTAL double, PARTE double);
/*Declare TotalA double;
Declare ParteA double;
Declare TotalB double;
Declare ParteB double;
Declare TotalC double;
Declare ParteC double;
Declare TotalD double;
Declare ParteD double;
Declare TotalE double;
Declare ParteE double;*/

--SELECT CASE WHEN :TipoFormula ='KG' THEN :Cantidad else  :KGExtrusion end INTO KGExtrusion FROM "DUMMY";

--convertirlo en funcion TABLA, hacer sum
formTemp= SELECT FO1."U_EXP_CODRUT" as "RUTA",'A'  as "TIPO",SUM(FO1."U_EXP_CAPA1")as "TOTAL", (EXP_GetKG(:ItemCode, 1000, 1, FO1."U_EXP_CODRUT")*(FO0."U_EXP_PRCAP1"/100)) as "PARTE"
	FROM  "@EXP_OFRM" "FO0"
     JOIN "@EXP_FORMUL" "FO1" ON FO0."Code"=FO1."Code"  AND NULLIF(FO1."U_EXP_CODRUT",'') IS NOT NULL
     WHERE FO0."Code"=:CodFormula 
     GROUP BY FO1."U_EXP_CODRUT",FO0."U_EXP_PRCAP1" 
UNION ALL
SELECT FO1."U_EXP_CODRUT" as "RUTA",'B' as "TIPO",SUM(FO1."U_EXP_CAPA2")as "TOTAL", (EXP_GetKG(:ItemCode, 1000, 1, FO1."U_EXP_CODRUT")*(FO0."U_EXP_PRCAP2"/100))   as "PARTE"
FROM  "@EXP_OFRM" "FO0"
     JOIN "@EXP_FORMUL" "FO1" ON FO0."Code"=FO1."Code"  AND NULLIF(FO1."U_EXP_CODRUT",'') IS NOT NULL
     WHERE FO0."Code"=:CodFormula
     GROUP BY  FO1."U_EXP_CODRUT",FO0."U_EXP_PRCAP2" 
     UNION ALL
SELECT FO1."U_EXP_CODRUT" as "RUTA",'C' as "TIPO",SUM(FO1."U_EXP_CAPA3")as "TOTAL",  (EXP_GetKG(:ItemCode, 1000, 1, FO1."U_EXP_CODRUT")*(FO0."U_EXP_PRCAP3"/100))    as "PARTE"
FROM  "@EXP_OFRM" "FO0"
     JOIN "@EXP_FORMUL" "FO1" ON FO0."Code"=FO1."Code"  AND NULLIF(FO1."U_EXP_CODRUT",'') IS NOT NULL
     WHERE FO0."Code"=:CodFormula
     GROUP BY FO1."U_EXP_CODRUT",FO0."U_EXP_PRCAP3" 
     UNION ALL
SELECT FO1."U_EXP_CODRUT" as "RUTA",'D' as "TIPO",SUM(FO1."U_EXP_CAPA4")as "TOTAL",  (EXP_GetKG(:ItemCode, 1000, 1, FO1."U_EXP_CODRUT")*(FO0."U_EXP_PRCAP4"/100))    as "PARTE"
FROM  "@EXP_OFRM" "FO0"
     JOIN "@EXP_FORMUL" "FO1" ON FO0."Code"=FO1."Code"  AND NULLIF(FO1."U_EXP_CODRUT",'') IS NOT NULL
     WHERE FO0."Code"=:CodFormula
     GROUP BY FO1."U_EXP_CODRUT",FO0."U_EXP_PRCAP4" 
     UNION ALL
SELECT FO1."U_EXP_CODRUT" as "RUTA",'E' as "TIPO",SUM(FO1."U_EXP_CAPA5") as "TOTAL",  (EXP_GetKG(:ItemCode, 1000, 1, FO1."U_EXP_CODRUT")*(FO0."U_EXP_PRCAP5"/100))    as "PARTE"
FROM  "@EXP_OFRM" "FO0"
     JOIN "@EXP_FORMUL" "FO1" ON FO0."Code"=FO1."Code" AND NULLIF(FO1."U_EXP_CODRUT",'') IS NOT NULL
     WHERE FO0."Code"=:CodFormula
     GROUP BY FO1."U_EXP_CODRUT",FO0."U_EXP_PRCAP5";
/*
SELECT SUM(FO1."U_EXP_CAPA1"), SUM(EXP_GetKGExtrusion(:ItemCode,1000,:Cantidad, FO1."U_EXP_CODRUT")*(FO0."U_EXP_PRCAP1"/100)) into TotalA,ParteA  
	FROM  "@EXP_OFRM" "FO0"
     JOIN "@EXP_FORMUL" "FO1" ON FO0."Code"=FO1."Code"
     WHERE FO0."Code"=:CodFormula
     GROUP BY FO0."U_EXP_PRCAP1" ;

SELECT SUM(FO1."U_EXP_CAPA2"), SUM(EXP_GetKGExtrusion(:ItemCode,1000,:Cantidad, FO1."U_EXP_CODRUT")*(FO0."U_EXP_PRCAP2"/100)) into TotalB,ParteB  
FROM  "@EXP_OFRM" "FO0"
     JOIN "@EXP_FORMUL" "FO1" ON FO0."Code"=FO1."Code"
     WHERE FO0."Code"=:CodFormula
     GROUP BY FO0."U_EXP_PRCAP2" ;
     
SELECT SUM(FO1."U_EXP_CAPA3"),  SUM(EXP_GetKGExtrusion(:ItemCode,1000,:Cantidad, FO1."U_EXP_CODRUT")*(FO0."U_EXP_PRCAP3"/100)) into TotalC,ParteC  
FROM  "@EXP_OFRM" "FO0"
     JOIN "@EXP_FORMUL" "FO1" ON FO0."Code"=FO1."Code"
     WHERE FO0."Code"=:CodFormula
     GROUP BY FO0."U_EXP_PRCAP3" ;
     
SELECT SUM(FO1."U_EXP_CAPA4"),  SUM(EXP_GetKGExtrusion(:ItemCode,1000,:Cantidad, FO1."U_EXP_CODRUT")*(FO0."U_EXP_PRCAP4"/100)) into TotalD,ParteD  
FROM  "@EXP_OFRM" "FO0"
     JOIN "@EXP_FORMUL" "FO1" ON FO0."Code"=FO1."Code"
     WHERE FO0."Code"=:CodFormula
     GROUP BY FO0."U_EXP_PRCAP4" ;
    
SELECT SUM(FO1."U_EXP_CAPA5"),  SUM(EXP_GetKGExtrusion(:ItemCode,1000,:Cantidad, FO1."U_EXP_CODRUT")*(FO0."U_EXP_PRCAP5"/100)) into TotalE,ParteE  
FROM  "@EXP_OFRM" "FO0"
     JOIN "@EXP_FORMUL" "FO1" ON FO0."Code"=FO1."Code"
     WHERE FO0."Code"=:CodFormula
     GROUP BY FO0."U_EXP_PRCAP5";
*/
	RETURN 	
	SELECT
	     	0 "TIPO"
		,	"RUTA"
		,	"ITEMCODE"
		,	"DESC"
		,	3 "ORD"
		,	round(SUM("CANT"),1) "CANT"
		,	'KG' "UNI"
	 FROM(
	    /* select 
     FO1."U_EXP_CODRUT" "RUTA"
 ,	FO1."U_EXP_CODART" "ITEMCODE"
 ,	ITML."ItemName" "DESC"
 ,	round((FO1."U_EXP_CAPA1" / TEM."TOTAL")*TEM."PARTE",2) "CANT"
    FROM  "@EXP_OFRM" "FO0"
JOIN "@EXP_FORMUL" "FO1" ON FO0."Code"=FO1."Code"
jOIN OITM "ITML" ON  FO1."U_EXP_CODART"=ITML."ItemCode"
join :formTemp "TEM" on FO1."U_EXP_CODRUT" =TEM."RUTA" and TEM."TIPO"='A'
WHERE FO0."Code"=:CodFormula  AND TEM."TOTAL">0
union all
 select 
     FO1."U_EXP_CODRUT" "RUTA"
 ,	FO1."U_EXP_CODART" "ITEMCODE"
 ,	ITML."ItemName" "DESC"
 ,	round((FO1."U_EXP_CAPA2" / TEM."TOTAL")*TEM."PARTE",2) "CANT"
    FROM  "@EXP_OFRM" "FO0"
JOIN "@EXP_FORMUL" "FO1" ON FO0."Code"=FO1."Code"
jOIN OITM "ITML" ON  FO1."U_EXP_CODART"=ITML."ItemCode"
join :formTemp "TEM" on FO1."U_EXP_CODRUT" =TEM."RUTA" and TEM."TIPO"='B'
WHERE FO0."Code"=:CodFormula AND TEM."TOTAL">0
union all
 select 
     FO1."U_EXP_CODRUT" "RUTA"
 ,	FO1."U_EXP_CODART" "ITEMCODE"
 ,	ITML."ItemName" "DESC"
 ,	round((FO1."U_EXP_CAPA3" / TEM."TOTAL")*TEM."PARTE",2) "CANT"
    FROM  "@EXP_OFRM" "FO0"
JOIN "@EXP_FORMUL" "FO1" ON FO0."Code"=FO1."Code"
jOIN OITM "ITML" ON  FO1."U_EXP_CODART"=ITML."ItemCode"
join :formTemp "TEM" on FO1."U_EXP_CODRUT" =TEM."RUTA" and TEM."TIPO"='C'
WHERE FO0."Code"=:CodFormula AND TEM."TOTAL">0
   union all
 select 
     FO1."U_EXP_CODRUT" "RUTA"
 ,	FO1."U_EXP_CODART" "ITEMCODE"
 ,	ITML."ItemName" "DESC"
 ,	round((FO1."U_EXP_CAPA4" / TEM."TOTAL")*TEM."PARTE",2) "CANT"
    FROM  "@EXP_OFRM" "FO0"
JOIN "@EXP_FORMUL" "FO1" ON FO0."Code"=FO1."Code"
jOIN OITM "ITML" ON  FO1."U_EXP_CODART"=ITML."ItemCode"
join :formTemp "TEM" on FO1."U_EXP_CODRUT" =TEM."RUTA" and TEM."TIPO"='D'
WHERE FO0."Code"=:CodFormula  AND TEM."TOTAL">0
   union all
 select 
     FO1."U_EXP_CODRUT" "RUTA"
 ,	FO1."U_EXP_CODART" "ITEMCODE"
 ,	ITML."ItemName" "DESC"
 ,	round((FO1."U_EXP_CAPA5" / TEM."TOTAL")*TEM."PARTE",2) "CANT"
    FROM  "@EXP_OFRM" "FO0"
JOIN "@EXP_FORMUL" "FO1" ON FO0."Code"=FO1."Code"
jOIN OITM "ITML" ON  FO1."U_EXP_CODART"=ITML."ItemCode"
join :formTemp "TEM" on FO1."U_EXP_CODRUT" =TEM."RUTA" and TEM."TIPO"='E'
WHERE FO0."Code"=:CodFormula AND TEM."TOTAL">0*/
	   select 
     	FO1."U_EXP_CODRUT" "RUTA"
	 ,	FO1."U_EXP_CODART" "ITEMCODE"
	 ,	ITML."ItemName" "DESC"
	 ,	round((
	 CASE 
	 	WHEN TEM."TIPO"='A' THEN FO1."U_EXP_CAPA1" 
	 	WHEN TEM."TIPO"='B' THEN FO1."U_EXP_CAPA2" 
	 	WHEN TEM."TIPO"='C' THEN FO1."U_EXP_CAPA3" 
	 	WHEN TEM."TIPO"='D' THEN FO1."U_EXP_CAPA4" 
	 	WHEN TEM."TIPO"='E' THEN FO1."U_EXP_CAPA5"  
	 	end
	 / TEM."TOTAL")*TEM."PARTE",2) "CANT"
	    FROM  "@EXP_OFRM" "FO0"
	JOIN "@EXP_FORMUL" "FO1" ON FO0."Code"=FO1."Code" AND NULLIF(FO1."U_EXP_CODRUT",'') IS NOT NULL
	jOIN OITM "ITML" ON  FO1."U_EXP_CODART"=ITML."ItemCode"
	join :formTemp "TEM" on FO1."U_EXP_CODRUT" =TEM."RUTA"
	WHERE FO0."Code"=:CodFormula  AND TEM."TOTAL">0
	          
	/*          
     select 
     FO1."U_EXP_CODRUT" "RUTA"
 ,	FO1."U_EXP_CODART" "ITEMCODE"
 ,	ITML."ItemName" "DESC"
 ,	round((FO1."U_EXP_CAPA1" / :TotalA)*:ParteA,2) "CANT"
    FROM  "@EXP_OFRM" "FO0"
JOIN "@EXP_FORMUL" "FO1" ON FO0."Code"=FO1."Code"
jOIN OITM "ITML" ON  FO1."U_EXP_CODART"=ITML."ItemCode"
WHERE FO0."Code"=:CodFormula  AND :TotalA>0
union all
 select 
     FO1."U_EXP_CODRUT" "RUTA"
 ,	FO1."U_EXP_CODART" "ITEMCODE"
 ,	ITML."ItemName" "DESC"
 ,	round((FO1."U_EXP_CAPA2" / :TotalB)*:ParteB,2) "CANT"
    FROM  "@EXP_OFRM" "FO0"
JOIN "@EXP_FORMUL" "FO1" ON FO0."Code"=FO1."Code"
jOIN OITM "ITML" ON  FO1."U_EXP_CODART"=ITML."ItemCode"
WHERE FO0."Code"=:CodFormula AND :TotalB>0
union all
 select 
     FO1."U_EXP_CODRUT" "RUTA"
 ,	FO1."U_EXP_CODART" "ITEMCODE"
 ,	ITML."ItemName" "DESC"
 ,	round((FO1."U_EXP_CAPA3" / :TotalC)*:ParteC,2) "CANT"
    FROM  "@EXP_OFRM" "FO0"
JOIN "@EXP_FORMUL" "FO1" ON FO0."Code"=FO1."Code"
jOIN OITM "ITML" ON  FO1."U_EXP_CODART"=ITML."ItemCode"
WHERE FO0."Code"=:CodFormula AND :TotalC>0
   union all
 select 
     FO1."U_EXP_CODRUT" "RUTA"
 ,	FO1."U_EXP_CODART" "ITEMCODE"
 ,	ITML."ItemName" "DESC"
 ,	round((FO1."U_EXP_CAPA4" / :TotalD)*:ParteD,3) "CANT"
    FROM  "@EXP_OFRM" "FO0"
JOIN "@EXP_FORMUL" "FO1" ON FO0."Code"=FO1."Code"
jOIN OITM "ITML" ON  FO1."U_EXP_CODART"=ITML."ItemCode"
WHERE FO0."Code"=:CodFormula AND :TotalD>0
   union all
 select 
     FO1."U_EXP_CODRUT" "RUTA"
 ,	FO1."U_EXP_CODART" "ITEMCODE"
 ,	ITML."ItemName" "DESC"
 ,	round((FO1."U_EXP_CAPA5" / :TotalE)*:ParteE,2) "CANT"
    FROM  "@EXP_OFRM" "FO0"
JOIN "@EXP_FORMUL" "FO1" ON FO0."Code"=FO1."Code"
jOIN OITM "ITML" ON  FO1."U_EXP_CODART"=ITML."ItemCode"
WHERE FO0."Code"=:CodFormula AND :TotalE>0
*/
	          
     ) GROUP BY "RUTA","ITEMCODE","DESC";
     

END;


