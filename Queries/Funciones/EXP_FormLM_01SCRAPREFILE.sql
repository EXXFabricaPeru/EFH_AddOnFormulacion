CREATE FUNCTION "EXP_FormLM_01SCRAPREFILE"
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
		,	FO1."U_EXP_SCRAP" "ITEMCODE"
		, 	'' "DESC"
		,	6 "ORD"
		,	-1 "CANT"
		,	'' "UNI"

           FROM  "@EXP_OFRM" "FO0"
     JOIN "@EXP_FFRUTA" "FO1" ON FO0."Code"=FO1."Code"
     WHERE FO0."Code"=:CodFormula AND FO1."U_EXP_SCRAP"  IS NOT NULL
UNION ALL
	select 
		0 "TIPO"
		,	FO1."LineId" "RUTA"
		,	FO1."U_EXP_REFILE" "ITEMCODE"
		, 	'' "DESC"
		,	6 "ORD"
		,	-1 "CANT"
		,	'' "UNI"
      FROM "@EXP_OFRM" "FO0" 
     JOIN "@EXP_FFRUTA" "FO1" ON FO0."Code"=FO1."Code"
     WHERE FO0."Code"=:CodFormula AND FO1."U_EXP_REFILE"  IS NOT NULL
     ;
     

END;