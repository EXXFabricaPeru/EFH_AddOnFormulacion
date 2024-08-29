CREATE FUNCTION "EXP_GetRutasKGDef"
(  
	in ItemCode NVARCHAR(50),
	in Ruta int
)  
returns TABLE 
(
	"RUTA" int
)
LANGUAGE SQLSCRIPT AS 
BEGIN  
	
RETURN 
	select*from(
	SELECT "RUTA" FROM "EXP_GetRutasKG"(:ItemCode) WHERE "TIPO"='EXT' AND "DEST" <=:Ruta
	union all
	SELECT "RUTA" FROM "EXP_GetRutasKG"(:ItemCode) WHERE "TIPO"='EXT' AND "RUTA" =:Ruta
	union all
	SELECT "RUTA" FROM "EXP_GetRutasKG"(:ItemCode) WHERE "TIPO"!='EXT' AND "RUTA" <=:Ruta
  );
end;