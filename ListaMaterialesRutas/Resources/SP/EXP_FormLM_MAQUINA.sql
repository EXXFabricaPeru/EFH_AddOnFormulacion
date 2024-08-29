CREATE FUNCTION "EXP_FormLM_MAQUINA" ( in codformula nvarchar(50),
                                IN itemcode   nvarchar(50),
                                IN metroimpresion DOUBLE,
                                IN kgextrusion DOUBLE,
                                IN cantidad DOUBLE )
  -- =============================================
  -- Autor           : Quevedo Grimaldo, Ricardo Francisco
  -- Fecha Creación  : 15/03/2021
  -- =============================================
  returns TABLE ( 	"TIPO" int, 
  					"RUTA" int, 
  					"ITEMCODE" nvarchar(50), 
  					"DESC" nvarchar(100), 
  					"ORD" int, 
  					"CANT" DOUBLE, 
  					"UNI" nvarchar(2), 
  					"CADI" DOUBLE ) language sqlscript AS
  BEGIN
	DECLARE HORA INT;  
    DECLARE HoraSel INT;  
	Declare LargoPT  double;
	Declare AnchoPT  double;	
	
	SELECT 60, 55 
    INTO HORA,HoraSel 
    FROM "DUMMY";
	
		SELECT TOP 1 DISTINCT		
		     	(((ITM."U_EXP_ANCHO"+IFNULL(ITM."U_EXP_REFANC",0))*IFNULL(NULLIF(ITM."U_EXP_FACANC",0),1))*"EXP_GetConversion"(ITM."U_EXP_UNDANCH")),--ANCHO PT
		     	CASE WHEN FO0."U_EXP_TIPO"='ME' THEN 1 ELSE  ((ITM."U_EXP_LARGO"*IFNULL(NULLIF(ITM."U_EXP_FACLAR",0),1)+IFNULL(ITM."U_EXP_REFLAR",0))*"EXP_GetConversion"(ITM."U_EXP_UNDLAR")) END--LARGO PT
	INTO AnchoPT, LargoPT 
	FROM OITM "ITM" 
	JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
	WHERE  ITM."ItemCode"=:ItemCode;
	
    RETURN
    SELECT 1 "TIPO" ,
           "RUTA" ,
           "ITEMCODE" ,
           "DESC" ,
           5 "ORD" ,
           ROUND(ifnull(NULLIF("CANT",0),1),2,ROUND_UP) "CANT" ,
           'HH' "UNI" ,
           "CADI"
    FROM  (
			--STANDARD  
                  SELECT fo1."U_EXP_CODRUT" "RUTA" ,
                         fo1."U_EXP_RECMAQ" "ITEMCODE" ,
                         res."ResName" "DESC" ,
                         --(EXP_GETKG(:ItemCode, 1000, 1, FO1."U_EXP_CODRUT")/(:AnchoPT*(IFNULL(NULLIF(FO1."U_EXP_ESPESOR",0),1)*"EXP_GetConversion"(FO1."U_EXP_UNESP"))*(IFNULL(MLIN."U_EXP_GCM33",1)*1000)))/
                         (EXP_GetMetros(:ItemCode, 1000, FO1."U_EXP_CODRUT"))/
                         ifnull(NULLIF(fo1."U_EXP_VELMAQ",0),ifnull(res."U_EXP_VELMAQ",1))/
                         :HORA
                         "CANT" ,
                         ifnull(fo1."U_EXP_TPOPRE",0) "CADI"
                  FROM   "@EXP_OFRM" "FO0"
                  JOIN   "@EXP_LAMINA" "FO1" ON fo0."Code"=fo1."Code"
                  LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FO1."U_EXP_MTRL"= MLIN."Name"                  
                  JOIN   "ORSC" "RES" ON fo1."U_EXP_RECMAQ"=res."ResCode"
                  JOIN   "@EXP_FFRUTA" "RU" ON fo1."U_EXP_CODRUT" =ru."LineId" AND ru."Code"=fo1."Code"
                  WHERE  fo0."Code"=:CodFormula
                  UNION ALL 
                  SELECT fo1."U_EXP_CODRUT" "RUTA" ,
                         fo1."U_EXP_RECMAQ" "ITEMCODE" ,
                         res."ResName" "DESC" ,
                        -- (EXP_GETKG(:ItemCode, 1000, 1, FO1."U_EXP_CODRUT")/(:AnchoPT*(IFNULL(NULLIF(FO1."U_EXP_ESPESOR",0),1)*"EXP_GetConversion"(FO1."U_EXP_UNESP"))*(IFNULL(MLIN."U_EXP_GCM33",1)*1000)))/
                         (EXP_GetMetros(:ItemCode, 1000, FO1."U_EXP_CODRUT"))/                      
                         ifnull(NULLIF(fo1."U_EXP_VELMAQ",0),ifnull(res."U_EXP_VELMAQ",1))/
                         :HORA
                         "CANT" ,
                         ifnull(fo1."U_EXP_TPOPRE",0) "CADI"
                  FROM   "@EXP_OFRM" "FO0"
                  JOIN   "@EXP_FRIMPR" "FO1" ON fo0."Code"=fo1."Code"
                  LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FO1."U_EXP_MTRL"= MLIN."Name"
                  JOIN   "ORSC" "RES" ON fo1."U_EXP_RECMAQ"=res."ResCode" 
                  JOIN   "@EXP_FFRUTA" "RU" ON fo1."U_EXP_CODRUT" =ru."LineId" AND ru."Code"=fo1."Code"
                  WHERE  fo0."Code"=:CodFormula
                  UNION ALL
                  SELECT fo1."U_EXP_CODRUT" "RUTA" ,
                         fo1."U_EXP_RECMAQ" "ITEMCODE" ,
                         res."ResName" "DESC" ,
                        -- (EXP_GETKG(:ItemCode, 1000, 1, FO1."U_EXP_CODRUT")/(:AnchoPT*(IFNULL(NULLIF(FO1."U_EXP_ESPESOR",0),1)*"EXP_GetConversion"(FO1."U_EXP_UNESP"))*(IFNULL(MLIN."U_EXP_GCM33",1)*1000)))/
                         (EXP_GetMetros(:ItemCode, 1000, FO1."U_EXP_CODRUT"))/                         
                         ifnull(NULLIF(fo1."U_EXP_VELMAQ",0),ifnull(res."U_EXP_VELMAQ",1))/
                         :HORA
                         "CANT" ,
                         ifnull(fo1."U_EXP_TPOPRE",0) "CADI"
                  FROM   "@EXP_OFRM" "FO0"
                  JOIN   "@EXP_FRCORT" "FO1" ON fo0."Code"=fo1."Code"
                  LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FO1."U_EXP_MTRL"= MLIN."Name"                  
                  JOIN   "ORSC" "RES" ON fo1."U_EXP_RECMAQ"=res."ResCode"
                  JOIN   "@EXP_FFRUTA" "RU" ON fo1."U_EXP_CODRUT" =ru."LineId" AND ru."Code"=fo1."Code"
                  WHERE  fo0."Code"=:CodFormula
                  UNION ALL
                  SELECT fo1."U_EXP_CODRUT" "RUTA" ,
                         fo1."U_EXP_RECMAQ" "ITEMCODE" ,
                         res."ResName" "DESC" ,
                         --(EXP_GETKG(:ItemCode, 1000, 1, FO1."U_EXP_CODRUT")/(:AnchoPT*(IFNULL(NULLIF(FO1."U_EXP_ESPESOR",0),1)*"EXP_GetConversion"(FO1."U_EXP_UNESP"))*(IFNULL(MLIN."U_EXP_GCM33",1)*1000)))/
                         (EXP_GetMetros(:ItemCode, 1000, FO1."U_EXP_CODRUT"))/                         
                         ifnull(NULLIF(fo1."U_EXP_VELMAQ",0),ifnull(res."U_EXP_VELMAQ",1))/
                         :HORA
                         "CANT" ,
                         ifnull(fo1."U_EXP_TPOPRE",0) "CADI"
                  FROM   "@EXP_OFRM" "FO0"
                  JOIN   "@EXP_FRHABI" "FO1" ON fo0."Code"=fo1."Code"
                  LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FO1."U_EXP_MTRL"= MLIN."Name"                  
                  JOIN   "ORSC" "RES" ON fo1."U_EXP_RECMAQ"=res."ResCode"
                  JOIN   "@EXP_FFRUTA" "RU" ON fo1."U_EXP_CODRUT" =ru."LineId" AND ru."Code"=fo1."Code"
                  WHERE  fo0."Code"=:CodFormula
                  UNION ALL
                  SELECT fo1."U_EXP_CODRUT" "RUTA" ,
                         fo1."U_EXP_RECMAQ" "ITEMCODE" ,
                         res."ResName" "DESC" ,
                         --(EXP_GETKG(:ItemCode, 1000, 1, FO1."U_EXP_CODRUT")/(:AnchoPT*(IFNULL(NULLIF(FO1."U_EXP_ESPESOR",0),1)*"EXP_GetConversion"(FO1."U_EXP_UNESP"))*(IFNULL(MLIN."U_EXP_GCM33",1)*1000)))/
                         (EXP_GetMetros(:ItemCode, 1000, FO1."U_EXP_CODRUT"))/                         
                         ifnull(NULLIF(fo1."U_EXP_VELMAQ",0),ifnull(res."U_EXP_VELMAQ",1))/
                         :HORA
                         "CANT" ,
                         ifnull(fo1."U_EXP_TPOPRE",0) "CADI"
                  FROM   "@EXP_OFRM" "FO0"
                  JOIN   "@EXP_FRREBO" "FO1" ON fo0."Code"=fo1."Code"
                  LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FO1."U_EXP_MTRL"= MLIN."Name"                  
                  JOIN   "ORSC" "RES" ON fo1."U_EXP_RECMAQ"=res."ResCode" 
                  JOIN   "@EXP_FFRUTA" "RU" ON fo1."U_EXP_CODRUT" =ru."LineId" AND ru."Code"=fo1."Code"
                  WHERE  fo0."Code"=:CodFormula
			--SELLADO                  
                  UNION ALL --MILLARES
                  SELECT fo1."U_EXP_CODRUT" "RUTA" ,
                         fo1."U_EXP_RECMAQ" "ITEMCODE" ,
                         res."ResName" "DESC" ,
                         ROUND(
                         	((:Cantidad*1000)/ifnull(FO1."U_EXP_CAIDA",1))/
                         	ifnull(NULLIF(FO1."U_EXP_VELMAQ",0),ifnull(RES."U_EXP_VELMAQ",1))/
                         	:HoraSel
                         ,2) "CANT" ,
                         ifnull(fo1."U_EXP_TPOPRE",0) "CADI"
                  FROM   "@EXP_OFRM" "FO0"
                  JOIN   "@EXP_FRSELA" "FO1" ON fo0."Code"=fo1."Code"
                  JOIN   "ORSC" "RES" ON fo1."U_EXP_RECMAQ"=res."ResCode"
                  JOIN   "@EXP_FFRUTA" "RU" ON fo1."U_EXP_CODRUT" =ru."LineId" AND ru."Code"=fo1."Code"
                  WHERE  fo0."Code"=:CodFormula AND FO0."U_EXP_TIPO"='CM'
                  
                  UNION ALL --OTROS
                  SELECT fo1."U_EXP_CODRUT" "RUTA" ,
                         fo1."U_EXP_RECMAQ" "ITEMCODE" ,
                         res."ResName" "DESC" ,
                         ROUND(
                         	((EXP_GETKG(:ItemCode, 1000, 1, FO1."U_EXP_CODRUT")/
                         		(:AnchoPT * 
                         		:LargoPT * 
                         		(IFNULL(NULLIF(FO1."U_EXP_ESPESOR",0),1)*"EXP_GetConversion"(FO1."U_EXP_UNESP"))* 
		     					(IFNULL(MLIN."U_EXP_GCM33",1)*1000) )
                         	)/ifnull(FO1."U_EXP_CAIDA",1))/
                         	ifnull(NULLIF(FO1."U_EXP_VELMAQ",0),ifnull(RES."U_EXP_VELMAQ",1))/
                         	:HoraSel
                         ,2) "CANT" ,
                         ifnull(fo1."U_EXP_TPOPRE",0) "CADI"
                  FROM   "@EXP_OFRM" "FO0"
                  JOIN   "@EXP_FRSELA" "FO1" ON fo0."Code"=fo1."Code"
                  LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FO1."U_EXP_MTRL"= MLIN."Name"
                  JOIN   "ORSC" "RES" ON fo1."U_EXP_RECMAQ"=res."ResCode"
                  JOIN   "@EXP_FFRUTA" "RU" ON fo1."U_EXP_CODRUT" =ru."LineId" AND ru."Code"=fo1."Code"
                  WHERE  fo0."Code"=:CodFormula AND FO0."U_EXP_TIPO"!='CM'
			--EXTRUSION
                  UNION ALL
                  SELECT fo1."U_EXP_CODRUTDE" "RUTA" ,
                         fo1."U_EXP_RECMAQ" "ITEMCODE" ,
                         res."ResName" "DESC" ,
                         EXP_GETKG(:ItemCode, 1000, 1, FO1."U_EXP_CODRUTDE")/(ifnull(NULLIF(fo1."U_EXP_VELMAQ",0),ifnull(res."U_EXP_VELMAQ",1))/1) "CANT" ,
                         ifnull(fo1."U_EXP_TPOPRE",0) "CADI"
                  FROM   "@EXP_OFRM" "FO0"
                  JOIN   "@EXP_FREXTR" "FO1" ON fo0."Code"=fo1."Code"
                  JOIN   "ORSC" "RES" ON fo1."U_EXP_RECMAQ"=res."ResCode"
                  JOIN   "@EXP_FFRUTA" "RU" ON fo1."U_EXP_CODRUTDE" =ru."LineId" AND ru."Code"=fo1."Code"
                  WHERE  fo0."Code"=:CodFormula
			);  
  end;


