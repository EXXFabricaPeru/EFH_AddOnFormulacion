CREATE PROCEDURE "EXP_FormulacionListaMateriales"
	(IN ItemCode NVARCHAR(50),
		IN UserDB NVARCHAR(25))
AS
BEGIN
Declare CodFormula nvarchar(50);
Declare TipoFormula nvarchar(2);
Declare Factor double;
Declare MetroImpresion double;
Declare KGTotal double;
Declare KGTotalMerma double;
Declare Cantidad double;
Declare tipoEnProduccion nvarchar(1);
Declare isCAST nvarchar(1);
----
Declare AlmacenCAST nvarchar(8);
Declare Almacen nvarchar(8);
Declare AlmacenMQ nvarchar(8);
Declare AlmacenInductores nvarchar(8);

	SELECT 1000, '106',CASE WHEN "U_EXX_CAST" ='Y' THEN '106' ELSE '101' END,'001', CASE WHEN "U_EXX_CAST" ='Y' THEN '120' ELSE '119' END,"U_EXX_CAST"
    INTO Factor,AlmacenCAST,Almacen,AlmacenMQ, AlmacenInductores,isCAST
    FROM "OUSR" WHERE "USER_CODE"=:UserDB;

    SELECT 	FO0."Code", 
    		FO0."U_EXP_TIPO", 
	        CASE WHEN FO0."U_EXP_TIPO"='CM' THEN FO0."U_EXP_QTT" 
	        WHEN FO0."U_EXP_TIPO"='ME' THEN FO0."U_EXP_QTT" ELSE 1 END, 	    
	    IFNULL(
          CASE WHEN FO0."U_EXP_TIPO"='CM' THEN  FO0."U_EXP_QTT" 
          ELSE FO0."U_EXP_QTT" END 
          *ITM."U_EXP_LARGO" *"EXP_GetConversion"(ITM."U_EXP_UNDLAR")
          ,1) --,
          --IFNULL(ITM."U_EXP_TPRO", '')
    INTO CodFormula,TipoFormula, Cantidad, MetroImpresion 
    FROM OITM "ITM" 
    JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
    WHERE "ItemCode"= :ItemCode;
    
	SELECT EXP_GetKGTotal(:ItemCode,:Factor,0),EXP_GetKGTotal(:ItemCode,:Factor,1)  
	INTO KGTotal,KGTotalMerma
	FROM dummy;	
              
	SELECT 
		:KGTotal "KILOSIN"
        ,:KGTotalMerma "KILO"
        ,CASE IFNULL(T0."U_EXP_TPRO", '') 
        	WHEN 'I' THEN AlmacenInductores 
        	ELSE	(CASE 
        				WHEN "TIPO"=1 THEN ifnull(ALM."U_EXP_WHS"||'',:AlmacenMQ) 
        				ELSE IFNULL((CASE WHEN :isCAST='Y' THEN ALM."U_EXP_WHSCAST" ELSE ALM."U_EXP_WHS" END)||'',:Almacen) 
        			END) 
        END  "WHS"
        , MAIN.*
        , PRED."PRED" 
        FROM(
		SELECT *, 0 "CADI", 0 "PRICE" FROM "EXP_FormLM_01RUTAS"(:CodFormula)	--RUTAS 0
		UNION ALL SELECT "TIPO","RUTA" ,"ITEMCODE" ,"DESC","ORD","CANT","UNI", 0 "CADI", 0 "PRICE" FROM "EXP_FormLM_MATERIAPRIMA"(:CodFormula,:ItemCode) --MateriaPrima  1
		UNION ALL SELECT "TIPO","RUTA" ,"ITEMCODE" ,"DESC","ORD","CANT","UNI", 0 "CADI", 0 "PRICE" FROM "EXP_FormLM_RESULTADO"(:CodFormula,:ItemCode,:Cantidad) -- ProductoEnProceso 2
		UNION ALL SELECT *, 0 "CADI", 0 "PRICE" FROM "EXP_FormLM_EXTRUSION"(:CodFormula,:ItemCode,:TipoFormula,:Cantidad) --FormulaExtrusion 3
		UNION ALL SELECT *, 0 "CADI", 0 "PRICE" FROM "EXP_FormLM_01INDUCTOR"(:CodFormula) --Inductor 4
		UNION ALL SELECT *, 0 "PRICE" FROM "EXP_FormLM_MAQUINA"(:CodFormula,:ItemCode,:MetroImpresion,:KGTotal,:Cantidad)--Maquina 5
		UNION ALL SELECT *, 0 "CADI", 0 "PRICE" FROM "EXP_FormLM_01SUBPRODUCTO"(:CodFormula)--SubProducto 6
		UNION ALL SELECT *, 0 "CADI", 0 "PRICE" FROM "EXP_FormLM_01SCRAPREFILE"(:CodFormula) --ScrapRefile 7
		UNION ALL SELECT * FROM "EXP_FormLM_SERVICIOS"(:CodFormula) --Servicio 0
	) "MAIN" 
	JOIN "EXP_GetRutasPRE"(:ItemCode) "PRED" ON MAIN."RUTA"=PRED."RUTA"
	JOIN "@EXP_FFRUTA" "RUT" ON MAIN."RUTA"=RUT."LineId" and RUT."Code"=:CodFormula
	LEFT JOIN "@EXP_ALMFOR" "ALM" ON RUT."U_EXP_CODRUT"=ALM."U_EXP_RUT" AND  MAIN."ORD"=ALM."U_EXP_ORD" AND RUT."U_EXP_CODRUT" IS NOT NULL
	LEFT JOIN OITM AS T0 ON MAIN."ITEMCODE" = T0."ItemCode"
	
	ORDER BY "RUTA", "ORD";
	
END;