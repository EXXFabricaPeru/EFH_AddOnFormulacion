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
	          

	          
     ) GROUP BY "RUTA","ITEMCODE","DESC";
     

END;