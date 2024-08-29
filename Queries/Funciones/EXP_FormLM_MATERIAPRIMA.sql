CREATE FUNCTION "EXP_FormLM_MATERIAPRIMA"
(
	IN CodFormula nvarchar(50),
	IN ItemCode nvarchar(50)
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
	"UNI" nvarchar(2),
"MERMA" double,	
	"TM" nvarchar(2)
	
)
LANGUAGE SQLSCRIPT
AS

BEGIN

	RETURN 	
	SELECT
	     	0 "TIPO"
		,	"RUTA"
		,	"ITEMCODE"
		,	"DESC"
		,	1 "ORD"
		,	"CANT"
		,	'KG' "UNI"
		, "MERMA"	 "MERMA"		
		,	"TipoMateria" "TM"
	 FROM(
	--MATERIAS PRIMAS
	---IMPRESION
     	SELECT 
			FO1."LineId" "RUTA"
		,	FO2."U_EXP_MPRIMA" "ITEMCODE"
		,	ITML."ItemName" "DESC"

			, EXP_GetKGArti(MAIN."ItemCode", 1000, 1, FO1."LineId",'') "CANT"
			,(IFNULL(FO2."U_EXP_MERMA",0)/100) "MERMA"			
		,	'MP'	"TipoMateria"
     FROM  "OITM" "MAIN"
     JOIN     "@EXP_OFRM" "FO0" ON MAIN."U_EXP_FORM"=FO0."Code"
     JOIN "@EXP_FFRUTA" "FO1" ON FO0."Code"=FO1."Code"
     JOIN "@EXP_FRIMPR" "FO2" ON FO0."Code"=FO2."Code" AND FO2."U_EXP_CODRUT"=FO1."LineId"
     jOIN OITM "ITML" ON  FO2."U_EXP_MPRIMA"=ITML."ItemCode"
     WHERE MAIN."ItemCode"=:ItemCode
     
     UNION ALL
	---LAMINA   1   
     	select 
			FO1."LineId" "RUTA"
		,	FO2."U_EXP_MPRIMA" "ITEMCODE"
		,	ITML."ItemName" "DESC"

						, EXP_GetKGArti(MAIN."ItemCode", 1000, 1, FO1."LineId",'1') "CANT"
			,(IFNULL(FO2."U_EXP_MERMA",0)/100) "MERMA"
		,	'MP'	"TipoMateria"
     FROM  "OITM" "MAIN"
     JOIN     "@EXP_OFRM" "FO0" ON MAIN."U_EXP_FORM"=FO0."Code"
     JOIN "@EXP_FFRUTA" "FO1" ON FO0."Code"=FO1."Code"
     JOIN "@EXP_LAMINA" "FO2" ON FO0."Code"=FO2."Code" AND FO2."U_EXP_CODRUT"=FO1."LineId"
     jOIN OITM "ITML" ON  FO2."U_EXP_MPRIMA"=ITML."ItemCode"

     WHERE MAIN."ItemCode"=:ItemCode
         
     UNION ALL
      ---LAMINADA 2
     select 
			FO1."LineId" "RUTA"
		,	FO2."U_EXP_MPRIM2" "ITEMCODE"
		,	ITML."ItemName" "DESC"
		, EXP_GetKGArti(MAIN."ItemCode", 1000, 1, FO1."LineId",'2') "CANT"
		,(IFNULL(FO2."U_EXP_MERMAM2",0)/100) "MERMA"
		,	'MP'	"TipoMateria"
     FROM  "OITM" "MAIN"
     JOIN     "@EXP_OFRM" "FO0" ON MAIN."U_EXP_FORM"=FO0."Code"
     JOIN "@EXP_FFRUTA" "FO1" ON FO0."Code"=FO1."Code"
     JOIN "@EXP_LAMINA" "FO2" ON FO0."Code"=FO2."Code" AND FO2."U_EXP_CODRUT"=FO1."LineId"
     jOIN OITM "ITML" ON  FO2."U_EXP_MPRIM2"=ITML."ItemCode"
     WHERE MAIN."ItemCode"=:ItemCode
         
     UNION ALL
     
   	---SELLADO
     select 
			FO1."LineId" "RUTA"
		,	FO2."U_EXP_MPRIMA" "ITEMCODE"
		,	ITML."ItemName" "DESC"

		, EXP_GetKGArti(MAIN."ItemCode", 1000, 1, FO1."LineId",'') "CANT"			
		,(IFNULL(FO2."U_EXP_MERMA",0)/100) "MERMA"			
		,	'MP'	"TipoMateria"
     FROM  "OITM" "MAIN"
     JOIN     "@EXP_OFRM" "FO0" ON MAIN."U_EXP_FORM"=FO0."Code"
     JOIN "@EXP_FFRUTA" "FO1" ON FO0."Code"=FO1."Code"
     JOIN "@EXP_FRSELA" "FO2" ON FO0."Code"=FO2."Code" AND FO2."U_EXP_CODRUT"=FO1."LineId"
     jOIN OITM "ITML" ON  FO2."U_EXP_MPRIMA"=ITML."ItemCode"
     WHERE MAIN."ItemCode"=:ItemCode
        UNION ALL
     
   	---CORTE
     select 
			FO1."LineId" "RUTA"
		,	FO2."U_EXP_MPRIMA" "ITEMCODE"
		,	ITML."ItemName" "DESC"

		, EXP_GetKGArti(MAIN."ItemCode", 1000, 1, FO1."LineId",'') "CANT"			
		,(IFNULL(FO2."U_EXP_MERMA",0)/100) "MERMA"			
		,	'MP'	"TipoMateria"
     FROM  "OITM" "MAIN"
     JOIN     "@EXP_OFRM" "FO0" ON MAIN."U_EXP_FORM"=FO0."Code"
     JOIN "@EXP_FFRUTA" "FO1" ON FO0."Code"=FO1."Code"
     JOIN "@EXP_FRCORT" "FO2" ON FO0."Code"=FO2."Code" AND FO2."U_EXP_CODRUT"=FO1."LineId"
     jOIN OITM "ITML" ON  FO2."U_EXP_MPRIMA"=ITML."ItemCode"
     WHERE MAIN."ItemCode"=:ItemCode
   

     UNION ALL
     	---HABILITADO
     select 
			FO1."LineId" "RUTA"
		,	FO2."U_EXP_MPRIMA" "ITEMCODE"
		,	ITML."ItemName" "DESC"

		, EXP_GetKGArti(MAIN."ItemCode", 1000, 1, FO1."LineId",'') "CANT"			
		,(IFNULL(FO2."U_EXP_MERMA",0)/100) "MERMA"			
		,	'MP'	"TipoMateria"
     FROM  "OITM" "MAIN"
     JOIN     "@EXP_OFRM" "FO0" ON MAIN."U_EXP_FORM"=FO0."Code"
     JOIN "@EXP_FFRUTA" "FO1" ON FO0."Code"=FO1."Code"
     JOIN "@EXP_FRHABI" "FO2" ON FO0."Code"=FO2."Code" AND FO2."U_EXP_CODRUT"=FO1."LineId"
     jOIN OITM "ITML" ON  FO2."U_EXP_MPRIMA"=ITML."ItemCode"
     WHERE MAIN."ItemCode"=:ItemCode
   

    UNION ALL
     
        	---REBOBINADO
     select 
			FO1."LineId" "RUTA"
		,	FO2."U_EXP_MPRIMA" "ITEMCODE"
		,	ITML."ItemName" "DESC"

		, EXP_GetKGArti(MAIN."ItemCode", 1000, 1, FO1."LineId",'') "CANT"			
		,(IFNULL(FO2."U_EXP_MERMA",0)/100) "MERMA"			
		,	'MP'	"TipoMateria"
     FROM  "OITM" "MAIN"
     JOIN     "@EXP_OFRM" "FO0" ON MAIN."U_EXP_FORM"=FO0."Code"
     JOIN "@EXP_FFRUTA" "FO1" ON FO0."Code"=FO1."Code"
     JOIN "@EXP_FRREBO" "FO2" ON FO0."Code"=FO2."Code" AND FO2."U_EXP_CODRUT"=FO1."LineId"
     jOIN OITM "ITML" ON  FO2."U_EXP_MPRIMA"=ITML."ItemCode"
     WHERE MAIN."ItemCode"=:ItemCode
   

     );
     
     

END;