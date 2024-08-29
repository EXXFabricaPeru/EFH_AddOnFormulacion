CREATE FUNCTION "EXP_FormLM_01INDUCTOR"
(
	IN CodFormula nvarchar(50)
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

	RETURN 	SELECT 
     		0 "TIPO"
		,	FO1."LineId" "RUTA"
		,	IND."U_EXP_INDCTR" "ITEMCODE"
		,	ITML."ItemName" "DESC"
		,	4 "ORD"
		,	1 "CANT"
		,	'' "UNI"
     FROM  "@EXP_OFRM" "FO0" 
     JOIN "@EXP_FFRUTA" "FO1" ON FO0."Code"=FO1."Code"
     JOIN "@EXP_INDUCT" "IND" ON IND."Code"=FO0."Code" AND IND."U_EXP_CODRUT"=FO1."LineId" AND IND."U_EXP_STATUS"='Y'
     jOIN OITM "ITML" ON  IND."U_EXP_INDCTR"=ITML."ItemCode"
     WHERE FO0."Code"=:CodFormula;
     

END;


