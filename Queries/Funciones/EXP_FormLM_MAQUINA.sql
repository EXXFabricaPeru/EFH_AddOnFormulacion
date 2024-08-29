CREATE FUNCTION "EXP_FormLM_MAQUINA"
(
	IN CodFormula nvarchar(50),
	IN MetroImpresion double,
	IN KGExtrusion double,
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

	RETURN 	
	SELECT
	     	1 "TIPO"
		,	"RUTA"
		,	"ITEMCODE"
		,	"DESC"
		,	4 "ORD"
		,	"CANT"
		,	'HH' "UNI"
	 FROM(
select 
			FO1."U_EXP_CODRUTDE" "RUTA"
		,	FO1."U_EXP_RECMAQ" "ITEMCODE"
		,	RES."ResName" "DESC"
		,	:KGExtrusion/(IFNULL(NULLIF(FO1."U_EXP_VELMAQ",0),IFNULL(RES."U_EXP_VELMAQ",1))/1) "CANT"
     FROM "@EXP_OFRM" "FO0"
     JOIN "@EXP_FREXTR" "FO1" ON FO0."Code"=FO1."Code"
     JOIN "ORSC" "RES" ON FO1."U_EXP_RECMAQ"=RES."ResCode"
     JOIN "@EXP_FFRUTA" "RU" ON FO1."U_EXP_CODRUTDE" =RU."LineId" and RU."Code"=FO1."Code"     
     WHERE FO0."Code"=:CodFormula
     UNION ALL
     	select 
			FO1."U_EXP_CODRUT" "RUTA"
		,	FO1."U_EXP_RECMAQ" "ITEMCODE"
		,	RES."ResName" "DESC"
		,	:MetroImpresion/(IFNULL(NULLIF(FO1."U_EXP_VELMAQ",0),IFNULL(RES."U_EXP_VELMAQ",1))/60) "CANT"
     FROM "@EXP_OFRM" "FO0" 
     JOIN "@EXP_FRIMPR" "FO1" ON FO0."Code"=FO1."Code"
     JOIN "ORSC" "RES" ON FO1."U_EXP_RECMAQ"=RES."ResCode"
    JOIN "@EXP_FFRUTA" "RU" ON FO1."U_EXP_CODRUT" =RU."LineId" and RU."Code"=FO1."Code"    
     WHERE FO0."Code"=:CodFormula
      UNION ALL
     	select 
			FO1."U_EXP_CODRUT" "RUTA"
		,	FO1."U_EXP_RECMAQ" "ITEMCODE"
		,	RES."ResName" "DESC"
		,	:MetroImpresion/(IFNULL(NULLIF(FO1."U_EXP_VELMAQ",0),IFNULL(RES."U_EXP_VELMAQ",1))/60) "CANT"
     FROM "@EXP_OFRM" "FO0" 
     JOIN "@EXP_LAMINA" "FO1" ON FO0."Code"=FO1."Code"
     JOIN "ORSC" "RES" ON FO1."U_EXP_RECMAQ"=RES."ResCode"
    JOIN "@EXP_FFRUTA" "RU" ON FO1."U_EXP_CODRUT" =RU."LineId" and RU."Code"=FO1."Code"     
     WHERE FO0."Code"=:CodFormula
           UNION ALL
     	select 
			FO1."U_EXP_CODRUT" "RUTA"
		,	FO1."U_EXP_RECMAQ" "ITEMCODE"
		,	RES."ResName" "DESC"
		,	:MetroImpresion/(IFNULL(NULLIF(FO1."U_EXP_VELMAQ",0),IFNULL(RES."U_EXP_VELMAQ",1))/60) "CANT"
     FROM "@EXP_OFRM" "FO0" 
     JOIN "@EXP_FRCORT" "FO1" ON FO0."Code"=FO1."Code"
     JOIN "ORSC" "RES" ON FO1."U_EXP_RECMAQ"=RES."ResCode"
    JOIN "@EXP_FFRUTA" "RU" ON FO1."U_EXP_CODRUT" =RU."LineId" and RU."Code"=FO1."Code"       
     WHERE FO0."Code"=:CodFormula
           UNION ALL
     	select 
			FO1."U_EXP_CODRUT" "RUTA"
		,	FO1."U_EXP_RECMAQ" "ITEMCODE"
		,	RES."ResName" "DESC"
		,	:MetroImpresion/(IFNULL(NULLIF(FO1."U_EXP_VELMAQ",0),IFNULL(RES."U_EXP_VELMAQ",1))/60) "CANT"
     FROM "@EXP_OFRM" "FO0"
     JOIN "@EXP_FRHABI" "FO1" ON FO0."Code"=FO1."Code"
     JOIN "ORSC" "RES" ON FO1."U_EXP_RECMAQ"=RES."ResCode"
    JOIN "@EXP_FFRUTA" "RU" ON FO1."U_EXP_CODRUT" =RU."LineId" and RU."Code"=FO1."Code"         
     WHERE FO0."Code"=:CodFormula
           UNION ALL
     select 
			FO1."U_EXP_CODRUT" "RUTA"
		,	FO1."U_EXP_RECMAQ" "ITEMCODE"
		,	RES."ResName" "DESC"
		,	:MetroImpresion/(IFNULL(NULLIF(FO1."U_EXP_VELMAQ",0),IFNULL(RES."U_EXP_VELMAQ",1))/60) "CANT"
     FROM "@EXP_OFRM" "FO0" 
     JOIN "@EXP_FRREBO" "FO1" ON FO0."Code"=FO1."Code"
     JOIN "ORSC" "RES" ON FO1."U_EXP_RECMAQ"=RES."ResCode"
    JOIN "@EXP_FFRUTA" "RU" ON FO1."U_EXP_CODRUT" =RU."LineId" and RU."Code"=FO1."Code"      
     WHERE FO0."Code"=:CodFormula
     UNION ALL
     	select 
			FO1."U_EXP_CODRUT" "RUTA"
		,	FO1."U_EXP_RECMAQ" "ITEMCODE"
		,	RES."ResName" "DESC"
		,	:Cantidad/(IFNULL(NULLIF(FO1."U_EXP_VELMAQ",0),IFNULL(RES."U_EXP_VELMAQ",1))/60) "CANT"
     FROM "@EXP_OFRM" "FO0"
     JOIN "@EXP_FRSELA" "FO1" ON FO0."Code"=FO1."Code"
     JOIN "ORSC" "RES" ON FO1."U_EXP_RECMAQ"=RES."ResCode"
    JOIN "@EXP_FFRUTA" "RU" ON FO1."U_EXP_CODRUT" =RU."LineId" and RU."Code"=FO1."Code"    
     WHERE FO0."Code"=:CodFormula
     
     );
     

END;