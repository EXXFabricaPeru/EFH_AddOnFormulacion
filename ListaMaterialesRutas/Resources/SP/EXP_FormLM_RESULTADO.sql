CREATE FUNCTION "EXP_FormLM_RESULTADO"
(
	IN CodFormula nvarchar(50),
	IN ItemCode nvarchar(50),
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
	"UNI" nvarchar(2),
	"TM" nvarchar(2),
	"MERMA" double
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
		,	2 "ORD"
		--,	ROUND(sum("CANT") OVER (ORDER BY "RUTA" )*"Merma") "CANT"
		,	"CANT" "CANT"		
		,	'KG' "UNI"
		,	'RO' "TM"
		,	"Merma" "MERMA"
	 FROM(

     --EXTRUSO DESTINO


     
     select 
			FO2."U_EXP_CDRTDE" "RUTA"
		,	FO1."U_EXP_SUBPRD" "ITEMCODE"
		,	'' "DESC"
		,	EXP_GetKG(:ItemCode, 1000, 1, FO1."LineId") "CANT"
		, 	FO2."U_EXP_MERMA" "Merma"
     FROM "@EXP_OFRM" "FO0"
     JOIN "@EXP_FREXTR" "FO2" ON FO0."Code"=FO2."Code"
      JOIN "@EXP_FFRUTA" "FO1" ON FO0."Code"=FO1."Code" AND FO2."U_EXP_CODRUTDE"=FO1."LineId"
           jOIN OITM "ITML" ON  FO1."U_EXP_SUBPRD"=ITML."ItemCode"
     WHERE FO0."Code"=:CodFormula AND FO2."U_EXP_CDRTDE" IS NOT NULL

		--RUTA ORIGEN
     UNION ALL
     
     	select 
			FO2."U_EXP_CODRUT" "RUTA"
		,	FO1."U_EXP_SUBPRD" "ITEMCODE"
		,	ITML."ItemName" "DESC"
		,	EXP_GetKG(:ItemCode, 1000, 1, FO1."LineId") "CANT"
		, 	FO2."U_EXP_MERMA" "Merma"		
		--,	IFNULL(MP."CANT",0) "CANT"
		--, 1-MP."MERMA" "Merma"
     FROM "@EXP_OFRM" "FO0"
     JOIN "@EXP_LAMINA" "FO2" ON FO0."Code"=FO2."Code"
     JOIN "@EXP_FFRUTA" "FO1" ON FO0."Code"=FO1."Code" AND FO2."U_EXP_CODRUTOR"=FO1."LineId"
  --   LEFT JOIN "EXP_FormLM_MATERIAPRIMA"(:CodFormula,:ItemCode) "MP" ON FO2."U_EXP_CODRUTOR"=MP."RUTA"
     jOIN OITM "ITML" ON  FO1."U_EXP_SUBPRD"=ITML."ItemCode"
     WHERE FO0."Code"=:CodFormula AND FO2."U_EXP_CODRUTOR" IS NOT NULL
          UNION ALL
     	select 
			FO2."U_EXP_CODRUT" "RUTA"
		,	FO1."U_EXP_SUBPRD" "ITEMCODE"
		,	ITML."ItemName" "DESC"
		,	EXP_GetKG(:ItemCode, 1000, 1, FO1."LineId") "CANT"
		, 	FO2."U_EXP_MERMA" "Merma"		
	--	,	IFNULL(MP."CANT",0) "CANT"
	--	,  IFNULL(1-MP."MERMA",0) "Merma"
     FROM "@EXP_OFRM" "FO0"
     JOIN "@EXP_FRSELA" "FO2" ON FO0."Code"=FO2."Code"
     JOIN "@EXP_FFRUTA" "FO1" ON FO0."Code"=FO1."Code" AND FO2."U_EXP_CODRUTOR"=FO1."LineId"
  --   LEFT JOIN "EXP_FormLM_MATERIAPRIMA"(:CodFormula,:ItemCode) "MP" ON FO2."U_EXP_CODRUTOR"=MP."RUTA"
     jOIN OITM "ITML" ON  FO1."U_EXP_SUBPRD"=ITML."ItemCode"
     WHERE FO0."Code"=:CodFormula AND FO2."U_EXP_CODRUTOR" IS NOT NULL
          UNION ALL
     	select 
			FO2."U_EXP_CODRUT" "RUTA"
		,	FO1."U_EXP_SUBPRD" "ITEMCODE"
		,	ITML."ItemName" "DESC"
		,	EXP_GetKG(:ItemCode, 1000, 1, FO1."LineId") "CANT"
		, 	FO2."U_EXP_MERMA" "Merma"		
	--	,	IFNULL(MP."CANT",0) "CANT"
	--	, 1-MP."MERMA" "Merma"
      FROM "@EXP_OFRM" "FO0"
     JOIN "@EXP_FRCORT" "FO2" ON FO0."Code"=FO2."Code"
     JOIN "@EXP_FFRUTA" "FO1" ON FO0."Code"=FO1."Code" AND FO2."U_EXP_CODRUTOR"=FO1."LineId"
   --  LEFT JOIN "EXP_FormLM_MATERIAPRIMA"(:CodFormula,:ItemCode) "MP" ON FO2."U_EXP_CODRUTOR"=MP."RUTA"
     jOIN OITM "ITML" ON  FO1."U_EXP_SUBPRD"=ITML."ItemCode"
     WHERE FO0."Code"=:CodFormula AND FO2."U_EXP_CODRUTOR" IS NOT NULL
        UNION ALL
     	select 
			FO2."U_EXP_CODRUT" "RUTA"
		,	FO1."U_EXP_SUBPRD" "ITEMCODE"
		,	ITML."ItemName" "DESC"
		,	EXP_GetKG(:ItemCode, 1000, 1, FO1."LineId") "CANT"
		, 	FO2."U_EXP_MERMA" "Merma"		
	--	,	IFNULL(MP."CANT",0) "CANT"
	--	,  1-MP."MERMA" "Merma"
           FROM "@EXP_OFRM" "FO0"
     JOIN "@EXP_FRHABI" "FO2" ON FO0."Code"=FO2."Code"
     JOIN "@EXP_FFRUTA" "FO1" ON FO0."Code"=FO1."Code" AND FO2."U_EXP_CODRUTOR"=FO1."LineId"
 --    LEFT JOIN "EXP_FormLM_MATERIAPRIMA"(:CodFormula,:ItemCode) "MP" ON FO2."U_EXP_CODRUTOR"=MP."RUTA"
     jOIN OITM "ITML" ON  FO1."U_EXP_SUBPRD"=ITML."ItemCode"      
     WHERE FO0."Code"=:CodFormula AND FO2."U_EXP_CODRUTOR" IS NOT NULL
               UNION ALL
     	select 
			FO2."U_EXP_CODRUT" "RUTA"
		,	FO1."U_EXP_SUBPRD" "ITEMCODE"
		,	ITML."ItemName" "DESC"
		,	EXP_GetKG(:ItemCode, 1000, 1, FO1."LineId") "CANT"
		, 	FO2."U_EXP_MERMA" "Merma"		
	--	,	IFNULL(MP."CANT",0) "CANT"
	--	,  1-MP."MERMA" "Merma"
           FROM "@EXP_OFRM" "FO0"
     JOIN "@EXP_FRREBO" "FO2" ON FO0."Code"=FO2."Code"
     JOIN "@EXP_FFRUTA" "FO1" ON FO0."Code"=FO1."Code" AND FO2."U_EXP_CODRUTOR"=FO1."LineId"
  --   LEFT JOIN "EXP_FormLM_MATERIAPRIMA"(:CodFormula,:ItemCode) "MP" ON FO2."U_EXP_CODRUTOR"=MP."RUTA"
     jOIN OITM "ITML" ON  FO1."U_EXP_SUBPRD"=ITML."ItemCode"
     WHERE FO0."Code"=:CodFormula AND FO2."U_EXP_CODRUTOR" IS NOT NULL
     )
     group by "RUTA" ,"ITEMCODE","DESC","Merma" ,"CANT"
order by "RUTA"
     
     ;
     

END;


