CREATE FUNCTION "EXP_GetRutasKG"
(  
	in ItemCode NVARCHAR(50)
)  
returns TABLE 
(
	"TIPO" nvarchar(3),
	"RUTA" int,
	"DEST" int

)
LANGUAGE SQLSCRIPT AS 
BEGIN  
	
RETURN 
SELECT*FROM(
	select FRU."U_EXP_CODRUT" "TIPO", FRU."LineId" "RUTA", EXT."U_EXP_CDRTDE" "DEST"
	FROM OITM "ITM" 
    JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code" 
	JOIN "@EXP_FFRUTA" "FRU" ON FO0."Code"=FRU."Code" 
	LEFT JOIN "@EXP_FREXTR" "EXT" ON  FO0."Code"=EXT."Code" AND FRU."LineId"=EXT."U_EXP_CODRUTDE"
	WHERE  ITM."ItemCode"=:ItemCode AND FRU."U_EXP_CODRUT" IN ('EXT') 
	UNION ALL
	select DISTINCT FRU."U_EXP_CODRUT" "TIPO", FRU."LineId" "RUTA", 99 "DEST"
	FROM OITM "ITM" 
    JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code" 
	JOIN "@EXP_FFRUTA" "FRU" ON FO0."Code"=FRU."Code" 
	LEFT JOIN "@EXP_FRIMPR" "IMP" ON  FO0."Code"=IMP."Code" AND FRU."LineId"=IMP."U_EXP_CODRUT" 	
	LEFT JOIN "@EXP_LAMINA" "LAM" ON  FO0."Code"=LAM."Code" AND FRU."LineId"=LAM."U_EXP_CODRUT" 	
	LEFT JOIN "@EXP_FRSELA" "SEL" ON  FO0."Code"=SEL."Code" AND FRU."LineId"=SEL."U_EXP_CODRUT" 		
	LEFT JOIN "@EXP_FRCORT" "COR" ON  FO0."Code"=COR."Code" AND FRU."LineId"=COR."U_EXP_CODRUT" 	
	LEFT JOIN "@EXP_FRHABI" "HAB" ON  FO0."Code"=HAB."Code" AND FRU."LineId"=HAB."U_EXP_CODRUT" 	
	LEFT JOIN "@EXP_FRREBO" "REB" ON  FO0."Code"=REB."Code" AND FRU."LineId"=REB."U_EXP_CODRUT" 	
				
	WHERE  ITM."ItemCode"=:ItemCode AND FRU."U_EXP_CODRUT" NOT IN ('EXT') 
	AND 
	(IMP."U_EXP_MPRIMA" IS NOT NULL
	OR LAM."U_EXP_MPRIMA" IS NOT NULL
	OR SEL."U_EXP_MPRIMA" IS NOT NULL
	OR COR."U_EXP_MPRIMA" IS NOT NULL
	OR HAB."U_EXP_MPRIMA" IS NOT NULL
	OR REB."U_EXP_MPRIMA" IS NOT NULL	
			)
);
  
end;