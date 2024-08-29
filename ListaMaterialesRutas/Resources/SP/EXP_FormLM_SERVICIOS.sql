CREATE FUNCTION "EXP_FormLM_SERVICIOS"
(
	IN CodFormula nvarchar(50)
)
-- =============================================
-- Autor           : Quevedo Grimaldo, Ricardo Francisco
-- Fecha Creación  : 29/12/2021
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
	"CADI" int,
	"PRICE" double
)
LANGUAGE SQLSCRIPT
AS

BEGIN

	RETURN 	SELECT 
		0 "TIPO"
		,	FO1."U_EXP_CODRUT" "RUTA" 
		,	FO1."U_EXP_SERV" "ITEMCODE"
		,	IT."ItemName" "DESC"
		,	0 "ORD"
		,	1 "CANT"
		,	'' "UNI"
		, 	0 "CADI"
		,	FO1."U_EXP_IMPORTE"  "PRICE"
     FROM "@EXP_OFRM" "FO0"
     JOIN "@EXP_FRSERV" "FO1" ON FO0."Code"=FO1."Code"
     JOIN OITM "IT" ON FO1."U_EXP_SERV"=IT."ItemCode"
     WHERE FO0."Code"=:CodFormula;

END;


