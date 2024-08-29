ALTER FUNCTION EXP_GetKG(in ItemCode NVARCHAR(50), in Factor double, in Merma int, in IsRuta int) 
 RETURNS KGCalculado Double 
 LANGUAGE SQLSCRIPT AS
 BEGIN
	Declare HaveLAM  int;
	Declare TipoForm  nvarchar(2);
	Declare LargoPT  double;
	Declare AnchoPT  double;	 

	/*SELECT TOP 1 "RUTA" into IsRuta FROM  "EXP_GetRutasKG"(:ItemCode)
	ORDER BY ABS( "RUTA" - :IsRuta); */

	SELECT COUNT(*) INTO HaveLAM	 
	FROM OITM "ITM" 
    JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code" 
	JOIN "@EXP_FFRUTA" "FRU" ON FO0."Code"=FRU."Code" 
	WHERE  ITM."ItemCode"=:ItemCode AND FRU."U_EXP_CODRUT" IN ('LAM') ;
	  
	SELECT TOP 1 DISTINCT		
		     	((ITM."U_EXP_ANCHO"*IFNULL(NULLIF(ITM."U_EXP_FACANC",0),1)+IFNULL(ITM."U_EXP_REFANC",0))*"EXP_GetConversion"(ITM."U_EXP_UNDANCH")),--ANCHO PT
		     	CASE WHEN FO0."U_EXP_TIPO"='ME' THEN 1 ELSE  ((IFNULL(NULLIF(ITM."U_EXP_LARGO",0),1)*IFNULL(NULLIF(ITM."U_EXP_FACLAR",0),1)+IFNULL(ITM."U_EXP_REFLAR",0))*"EXP_GetConversion"(ITM."U_EXP_UNDLAR")) END,--LARGO PT
	   (CASE WHEN FO0."U_EXP_TIPO"='PK' THEN FO0."U_EXP_QTT" ELSE 0 END)
	   ,FO0."U_EXP_TIPO"
	INTO AnchoPT, LargoPT, KGCalculado ,TipoForm 
	FROM OITM "ITM" 
	JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
	WHERE  ITM."ItemCode"=:ItemCode;

	
	IF(:TipoForm='CM')THEN --CANT. MILLARES

 		IF(:HaveLAM>0) THEN		
	    SELECT SUM("KG") 
    	INTO KGCalculado  
     	FROM(
		     SELECT DISTINCT		
		     	:AnchoPT*--ANCHO PT
		     	:LargoPT*--LARGO PT
		     	(FLIN."U_EXP_ESPESOR"*"EXP_GetConversion"(FLIN."U_EXP_UNESP"))* -- ESPE ETAPA
		     	(IFNULL(MLIN."U_EXP_GCM33",1)*:Factor)* -- material
		     	(FO0."U_EXP_QTT"*1000) --CantidadBase
				*CASE WHEN(:Merma=1) THEN (1+IFNULL(FLIN."U_EXP_MERMA",0)/100) ELSE 1 END "KG"		     	 
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"
    		 LEFT  JOIN "@EXP_FRIMPR" "FLIN" ON FLIN."Code"=FO0."Code"  and FLIN."U_EXP_CODRUT"=RUT."LineId"
		     LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FLIN."U_EXP_MTRL"= MLIN."Name"
		     WHERE "ItemCode"=:ItemCode AND FLIN."U_EXP_MPRIMA" IS NOT NULL AND RUT."U_EXP_CODRUT" IN ('IMP') 
		     and  RUT."LineId" IN(select * from  "EXP_GetRutasKGDef"(:ItemCode,:IsRuta)) 
		     UNION 	
		     SELECT DISTINCT		
		     	:AnchoPT*--ANCHO PT
		     	:LargoPT*--LARGO PT
		     	(FO0."U_EXP_QTT"*1000)* --CantidadBase
		     	(((FLIN."U_EXP_ESPESOR"*"EXP_GetConversion"(FLIN."U_EXP_UNESP"))* -- ESPE ETAPA
		     	(IFNULL(MLIN."U_EXP_GCM33",1)*:Factor)
		     	*CASE WHEN(:Merma=1) THEN (1+IFNULL(FLIN."U_EXP_MERMA",0)/100) ELSE 1 END ) -- material
				+case when( FLIN."U_EXP_MPRIM2" IS NOT NULL) THEN ((FLIN."U_EXP_ESPESM2"*"EXP_GetConversion"(FLIN."U_EXP_UNESPM2"))* -- ESPE ETAPA
		     	(IFNULL(MLIN2."U_EXP_GCM33",1)*:Factor)) ELSE 0 END -- material
				*CASE WHEN(:Merma=1) THEN (1+IFNULL(FLIN."U_EXP_MERMAM2",0)/100) ELSE 1 END 
				)
				"KG"		        	 
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"
    		 LEFT  JOIN "@EXP_LAMINA" "FLIN" ON FLIN."Code"=FO0."Code"  and FLIN."U_EXP_CODRUT"=RUT."LineId"
		     LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FLIN."U_EXP_MTRL"= MLIN."Name"
   		     LEFT JOIN "@EXP_TIPMAT" "MLIN2" ON FLIN."U_EXP_MTRLM2"= MLIN2."Name"		     
		     WHERE "ItemCode"=:ItemCode AND FLIN."U_EXP_MPRIMA" IS NOT NULL AND RUT."U_EXP_CODRUT" IN ('LAM')
		     and  RUT."LineId" IN(select * from  "EXP_GetRutasKGDef"(:ItemCode,:IsRuta))
		     UNION 	
		          SELECT DISTINCT		
		     	:AnchoPT*--ANCHO PT
		     	:LargoPT*--LARGO PT
		     	(FLIN."U_EXP_ESPESOR"*"EXP_GetConversion"(FLIN."U_EXP_UNESP"))* -- ESPE ETAPA
		     	(IFNULL(MLIN."U_EXP_GCM33",1)*:Factor)* -- material
		     	(FO0."U_EXP_QTT"*1000) --CantidadBase
				*CASE WHEN(:Merma=1) THEN (1+IFNULL(FLIN."U_EXP_MERMA",0)/100) ELSE 1 END "KG"		     	 
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"
    		 LEFT  JOIN "@EXP_FRCORT" "FLIN" ON FLIN."Code"=FO0."Code"  and FLIN."U_EXP_CODRUT"=RUT."LineId"
		     LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FLIN."U_EXP_MTRL"= MLIN."Name"
		     WHERE "ItemCode"=:ItemCode AND FLIN."U_EXP_MPRIMA" IS NOT NULL AND RUT."U_EXP_CODRUT" IN ('COR')  
		     and  RUT."LineId" IN(select * from  "EXP_GetRutasKGDef"(:ItemCode,:IsRuta))		     
			UNION
		     SELECT DISTINCT		
		     	:AnchoPT*--ANCHO PT
		     	:LargoPT*--LARGO PT
		     	(FLIN."U_EXP_ESPESOR"*"EXP_GetConversion"(FLIN."U_EXP_UNESP"))* -- ESPE ETAPA
		     	(IFNULL(MLIN."U_EXP_GCM33",1)*:Factor)* -- material
		     	(FO0."U_EXP_QTT"*1000) --CantidadBase
				*CASE WHEN(:Merma=1) THEN (1+IFNULL(FLIN."U_EXP_MERMA",0)/100) ELSE 1 END "KG"		     	 
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"
    		 LEFT  JOIN "@EXP_FRSELA" "FLIN" ON FLIN."Code"=FO0."Code"  and FLIN."U_EXP_CODRUT"=RUT."LineId"
		     LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FLIN."U_EXP_MTRL"= MLIN."Name"
		     WHERE "ItemCode"=:ItemCode AND FLIN."U_EXP_MPRIMA" IS NOT NULL AND RUT."U_EXP_CODRUT" IN ('SEL')  
		     and  RUT."LineId" IN(select * from  "EXP_GetRutasKGDef"(:ItemCode,:IsRuta))		     
			UNION --EXTRUSION DE LAMINA
			SELECT 		
		     	:AnchoPT*--ANCHO PT
		     	:LargoPT*--LARGO PT
		     	(FLIN."U_EXP_ESPESOR"*"EXP_GetConversion"(FLIN."U_EXP_UNESP"))* -- ESPE ETAPA
		     	(IFNULL(MLIN."U_EXP_GCM33",1)*:Factor)* -- material
		     	(FO0."U_EXP_QTT"*1000) --CantidadBase
				*CASE WHEN(:Merma=1) THEN (1+IFNULL(FLIN."U_EXP_MERMA",0)/100) ELSE 1 END "KG"		     	 
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"
    		 LEFT  JOIN "@EXP_FREXTR" "FLIN" ON FLIN."Code"=FO0."Code"  and FLIN."U_EXP_CODRUTDE"=RUT."LineId"
		     LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FLIN."U_EXP_MATE"= MLIN."Name"
		     WHERE "ItemCode"=:ItemCode AND RUT."U_EXP_CODRUT" IN ('EXT')   
		     and  RUT."LineId" IN(select * from  "EXP_GetRutasKGDef"(:ItemCode,:IsRuta))	
			          		     
		     );

		     	  ELSE
		     	  
	    SELECT SUM("KG") 
    	INTO KGCalculado  
     	FROM(
		     SELECT DISTINCT		
		     	:AnchoPT*--ANCHO PT
		     	:LargoPT*--LARGO PT
		     	(FLIN."U_EXP_ESPESOR"*"EXP_GetConversion"(FLIN."U_EXP_UNESP"))* -- ESPE ETAPA
		     	(IFNULL(MLIN."U_EXP_GCM33",1)*:Factor)* -- material
		     	(FO0."U_EXP_QTT"*1000) --CantidadBase
				*CASE WHEN(:Merma=1) THEN (1+IFNULL(FLIN."U_EXP_MERMA",0)/100) ELSE 1 END "KG"		     	 
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"
    		 LEFT  JOIN "@EXP_FRIMPR" "FLIN" ON FLIN."Code"=FO0."Code"  and FLIN."U_EXP_CODRUT"=RUT."LineId"
		     LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FLIN."U_EXP_MTRL"= MLIN."Name"
		     WHERE "ItemCode"=:ItemCode AND FLIN."U_EXP_MPRIMA" IS NOT NULL AND RUT."U_EXP_CODRUT" IN ('IMP')  
		     and  RUT."LineId" IN(select * from  "EXP_GetRutasKGDef"(:ItemCode,:IsRuta))
		     UNION 	
		     SELECT DISTINCT		
		     	:AnchoPT*--ANCHO PT
		     	:LargoPT*--LARGO PT
		     	(FO0."U_EXP_QTT"*1000)* --CantidadBase
		     	(((FLIN."U_EXP_ESPESOR"*"EXP_GetConversion"(FLIN."U_EXP_UNESP"))* -- ESPE ETAPA
		     	(IFNULL(MLIN."U_EXP_GCM33",1)*:Factor)
		     	*CASE WHEN(:Merma=1) THEN (1+IFNULL(FLIN."U_EXP_MERMA",0)/100) ELSE 1 END ) -- material
				+case when( FLIN."U_EXP_MPRIM2" IS NOT NULL) THEN ((FLIN."U_EXP_ESPESM2"*"EXP_GetConversion"(FLIN."U_EXP_UNESPM2"))* -- ESPE ETAPA
		     	(IFNULL(MLIN2."U_EXP_GCM33",1)*:Factor)) ELSE 0 END -- material
				*CASE WHEN(:Merma=1) THEN (1+IFNULL(FLIN."U_EXP_MERMAM2",0)/100) ELSE 1 END 
				)
				"KG"		   	     	 
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"
    		 LEFT  JOIN "@EXP_LAMINA" "FLIN" ON FLIN."Code"=FO0."Code"  and FLIN."U_EXP_CODRUT"=RUT."LineId"
		     LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FLIN."U_EXP_MTRL"= MLIN."Name"
   		     LEFT JOIN "@EXP_TIPMAT" "MLIN2" ON FLIN."U_EXP_MTRLM2"= MLIN2."Name"
		     WHERE "ItemCode"=:ItemCode AND FLIN."U_EXP_MPRIMA" IS NOT NULL AND RUT."U_EXP_CODRUT" IN ('LAM') 
		     and  RUT."LineId" IN(select * from  "EXP_GetRutasKGDef"(:ItemCode,:IsRuta))  
		     UNION 	
			SELECT DISTINCT		
		     	:AnchoPT*--ANCHO PT
		     	:LargoPT*--LARGO PT
		     	(FLIN."U_EXP_ESPESOR"*"EXP_GetConversion"(FLIN."U_EXP_UNESP"))* -- ESPE ETAPA
		     	(IFNULL(MLIN."U_EXP_GCM33",1)*:Factor)* -- material
		     	(FO0."U_EXP_QTT"*1000) --CantidadBase
				*CASE WHEN(:Merma=1) THEN (1+IFNULL(FLIN."U_EXP_MERMA",0)/100) ELSE 1 END "KG"		     	 
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"
    		 LEFT  JOIN "@EXP_FRCORT" "FLIN" ON FLIN."Code"=FO0."Code"  --OJO
		     LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FLIN."U_EXP_MTRL"= MLIN."Name"
		     WHERE "ItemCode"=:ItemCode AND FLIN."U_EXP_MPRIMA" IS NOT NULL AND RUT."U_EXP_CODRUT" IN ('COR')  
		     and  RUT."LineId" IN(select * from  "EXP_GetRutasKGDef"(:ItemCode,:IsRuta))	     
			UNION		     
		     SELECT DISTINCT		
		     	:AnchoPT*--ANCHO PT
		     	:LargoPT*--LARGO PT
		     	(FLIN."U_EXP_ESPESOR"*"EXP_GetConversion"(FLIN."U_EXP_UNESP"))* -- ESPE ETAPA
		     	(IFNULL(MLIN."U_EXP_GCM33",1)*:Factor)* -- material
		     	(FO0."U_EXP_QTT"*1000) --CantidadBase
				*CASE WHEN(:Merma=1) THEN (1+IFNULL(FLIN."U_EXP_MERMA",0)/100) ELSE 1 END "KG"		     	 
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"
    		 LEFT  JOIN "@EXP_FRSELA" "FLIN" ON FLIN."Code"=FO0."Code"  --OJO
		     LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FLIN."U_EXP_MTRL"= MLIN."Name"
		     WHERE "ItemCode"=:ItemCode AND FLIN."U_EXP_MPRIMA" IS NOT NULL AND RUT."U_EXP_CODRUT" IN ('SEL')  
		     and  RUT."LineId" IN(select * from  "EXP_GetRutasKGDef"(:ItemCode,:IsRuta))	     
			UNION --EXTRUSION DE LAMINA
			SELECT DISTINCT		
		     	:AnchoPT*--ANCHO PT
		     	:LargoPT*--LARGO PT
		     	(FLIN."U_EXP_ESPESOR"*"EXP_GetConversion"(FLIN."U_EXP_UNESP"))* -- ESPE ETAPA
		     	(IFNULL(MLIN."U_EXP_GCM33",1)*:Factor)* -- material
		     	(FO0."U_EXP_QTT"*1000) --CantidadBase
				*CASE WHEN(:Merma=1) THEN (1+IFNULL(FLIN."U_EXP_MERMA",0)/100) ELSE 1 END "KG"		     	 
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"
    		 LEFT  JOIN "@EXP_FREXTR" "FLIN" ON FLIN."Code"=FO0."Code"  and FLIN."U_EXP_CODRUTDE"=RUT."LineId"
		     LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FLIN."U_EXP_MATE"= MLIN."Name"
		     WHERE "ItemCode"=:ItemCode AND RUT."U_EXP_CODRUT" IN ('EXT') 
		     and  RUT."LineId" IN(select * from  "EXP_GetRutasKGDef"(:ItemCode,:IsRuta))	
			          		     
		     );


	     END IF;

	ELSEIF(:TipoForm='ME') THEN	-- MED. METROSS
 		IF(:HaveLAM>0) THEN		
	    SELECT SUM("KG") 
    	INTO KGCalculado  
     	FROM(
		     SELECT DISTINCT		
		     	:AnchoPT*--ANCHO PT
		     	:LargoPT*--LARGO PT
		     	(FLIN."U_EXP_ESPESOR"*"EXP_GetConversion"(FLIN."U_EXP_UNESP"))* -- ESPE ETAPA
		     	(IFNULL(MLIN."U_EXP_GCM33",1)*:Factor)* -- material
		     	(FO0."U_EXP_QTT") --CantidadBase
				*CASE WHEN(:Merma=1) THEN (1+IFNULL(FLIN."U_EXP_MERMA",0)/100) ELSE 1 END "KG"		     	 
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"
    		 LEFT  JOIN "@EXP_FRIMPR" "FLIN" ON FLIN."Code"=FO0."Code"   and FLIN."U_EXP_CODRUT"=RUT."LineId"
		     LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FLIN."U_EXP_MTRL"= MLIN."Name"
		     WHERE "ItemCode"=:ItemCode AND FLIN."U_EXP_MPRIMA" IS NOT NULL AND RUT."U_EXP_CODRUT" IN ('IMP') 
		     and  RUT."LineId" IN(select * from  "EXP_GetRutasKGDef"(:ItemCode,:IsRuta))
		     UNION 	
		     SELECT DISTINCT		
		     	:AnchoPT*--ANCHO PT
		     	:LargoPT*--LARGO PT
		     	(FO0."U_EXP_QTT")* --CantidadBase
		     	(((FLIN."U_EXP_ESPESOR"*"EXP_GetConversion"(FLIN."U_EXP_UNESP"))* -- ESPE ETAPA
		     	(IFNULL(MLIN."U_EXP_GCM33",1)*:Factor)
		     	*CASE WHEN(:Merma=1) THEN (1+IFNULL(FLIN."U_EXP_MERMA",0)/100) ELSE 1 END ) -- material
				+case when( FLIN."U_EXP_MPRIM2" IS NOT NULL) THEN ((FLIN."U_EXP_ESPESM2"*"EXP_GetConversion"(FLIN."U_EXP_UNESPM2"))* -- ESPE ETAPA
		     	(IFNULL(MLIN2."U_EXP_GCM33",1)*:Factor)) ELSE 0 END -- material
				*CASE WHEN(:Merma=1) THEN (1+IFNULL(FLIN."U_EXP_MERMAM2",0)/100) ELSE 1 END 
				)
				"KG"		     	 
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"
    		 LEFT  JOIN "@EXP_LAMINA" "FLIN" ON FLIN."Code"=FO0."Code"   and FLIN."U_EXP_CODRUT"=RUT."LineId"
		     LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FLIN."U_EXP_MTRL"= MLIN."Name"
   		     LEFT JOIN "@EXP_TIPMAT" "MLIN2" ON FLIN."U_EXP_MTRLM2"= MLIN2."Name"
		     WHERE "ItemCode"=:ItemCode AND FLIN."U_EXP_MPRIMA" IS NOT NULL AND RUT."U_EXP_CODRUT" IN ('LAM') 
		     and  RUT."LineId" IN(select * from  "EXP_GetRutasKGDef"(:ItemCode,:IsRuta)) 
		     UNION 	
		     SELECT DISTINCT		
		     	:AnchoPT*--ANCHO PT
		     	:LargoPT*--LARGO PT
		     	(FLIN."U_EXP_ESPESOR"*"EXP_GetConversion"(FLIN."U_EXP_UNESP"))* -- ESPE ETAPA
		     	(IFNULL(MLIN."U_EXP_GCM33",1)*:Factor)* -- material
		     	(FO0."U_EXP_QTT") --CantidadBase
				*CASE WHEN(:Merma=1) THEN (1+IFNULL(FLIN."U_EXP_MERMA",0)/100) ELSE 1 END "KG"		     	 
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"
    		 LEFT  JOIN "@EXP_FRSELA" "FLIN" ON FLIN."Code"=FO0."Code"   and FLIN."U_EXP_CODRUT"=RUT."LineId"
		     LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FLIN."U_EXP_MTRL"= MLIN."Name"
		     WHERE "ItemCode"=:ItemCode AND FLIN."U_EXP_MPRIMA" IS NOT NULL AND RUT."U_EXP_CODRUT" IN ('SEL') 
		     and  RUT."LineId" IN(select * from  "EXP_GetRutasKGDef"(:ItemCode,:IsRuta))		     
			UNION --EXTRUSION DE LAMINA
			SELECT DISTINCT		
		     	:AnchoPT*--ANCHO PT
		     	:LargoPT*--LARGO PT
		     	(FLIN."U_EXP_ESPESOR"*"EXP_GetConversion"(FLIN."U_EXP_UNESP"))* -- ESPE ETAPA
		     	(IFNULL(MLIN."U_EXP_GCM33",1)*:Factor)* -- material
		     	(FO0."U_EXP_QTT") --CantidadBase
				*CASE WHEN(:Merma=1) THEN (1+IFNULL(FLIN."U_EXP_MERMA",0)/100) ELSE 1 END "KG"		     	 
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"
    		 LEFT  JOIN "@EXP_FREXTR" "FLIN" ON FLIN."Code"=FO0."Code"  and FLIN."U_EXP_CODRUTDE"=RUT."LineId"
		     LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FLIN."U_EXP_MATE"= MLIN."Name"
		     WHERE "ItemCode"=:ItemCode AND RUT."U_EXP_CODRUT" IN ('EXT')  
		     and  RUT."LineId" IN(select * from  "EXP_GetRutasKGDef"(:ItemCode,:IsRuta))
			          		     
		     );

		     	  ELSE
	    SELECT SUM("KG") 
    	INTO KGCalculado  
     	FROM(
		     SELECT DISTINCT		
		     	:AnchoPT*--ANCHO PT
		     	:LargoPT*--LARGO PT
		     	(FLIN."U_EXP_ESPESOR"*"EXP_GetConversion"(FLIN."U_EXP_UNESP"))* -- ESPE ETAPA
		     	(IFNULL(MLIN."U_EXP_GCM33",1)*:Factor)* -- material
		     	(FO0."U_EXP_QTT") --CantidadBase
				*CASE WHEN(:Merma=1) THEN (1+IFNULL(FLIN."U_EXP_MERMA",0)/100) ELSE 1 END "KG"		     	 
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"
    		 LEFT  JOIN "@EXP_FRIMPR" "FLIN" ON FLIN."Code"=FO0."Code"   and FLIN."U_EXP_CODRUT"=RUT."LineId"
		     LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FLIN."U_EXP_MTRL"= MLIN."Name"
		     WHERE "ItemCode"=:ItemCode AND FLIN."U_EXP_MPRIMA" IS NOT NULL AND RUT."U_EXP_CODRUT" IN ('IMP')  
		     and  RUT."LineId" IN(select * from  "EXP_GetRutasKGDef"(:ItemCode,:IsRuta))
		     UNION 	
		     SELECT DISTINCT		
		     	:AnchoPT*--ANCHO PT
		     	:LargoPT*--LARGO PT
		     	(FO0."U_EXP_QTT")* --CantidadBase
		     	(((FLIN."U_EXP_ESPESOR"*"EXP_GetConversion"(FLIN."U_EXP_UNESP"))* -- ESPE ETAPA
		     	(IFNULL(MLIN."U_EXP_GCM33",1)*:Factor)
		     	*CASE WHEN(:Merma=1) THEN (1+IFNULL(FLIN."U_EXP_MERMA",0)/100) ELSE 1 END ) -- material
				+case when( FLIN."U_EXP_MPRIM2" IS NOT NULL) THEN ((FLIN."U_EXP_ESPESM2"*"EXP_GetConversion"(FLIN."U_EXP_UNESPM2"))* -- ESPE ETAPA
		     	(IFNULL(MLIN2."U_EXP_GCM33",1)*:Factor)) ELSE 0 END -- material
				*CASE WHEN(:Merma=1) THEN (1+IFNULL(FLIN."U_EXP_MERMAM2",0)/100) ELSE 1 END 
				)
				"KG"		     	 
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"
    		 LEFT  JOIN "@EXP_LAMINA" "FLIN" ON FLIN."Code"=FO0."Code"   and FLIN."U_EXP_CODRUT"=RUT."LineId"
		     LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FLIN."U_EXP_MTRL"= MLIN."Name"
   		     LEFT JOIN "@EXP_TIPMAT" "MLIN2" ON FLIN."U_EXP_MTRLM2"= MLIN2."Name"
		     WHERE "ItemCode"=:ItemCode AND FLIN."U_EXP_MPRIMA" IS NOT NULL AND RUT."U_EXP_CODRUT" IN ('LAM') 
		     and  RUT."LineId" IN(select * from  "EXP_GetRutasKGDef"(:ItemCode,:IsRuta))
		     UNION 	
		     SELECT DISTINCT		
		     	:AnchoPT*--ANCHO PT
		     	:LargoPT*--LARGO PT
		     	(FLIN."U_EXP_ESPESOR"*"EXP_GetConversion"(FLIN."U_EXP_UNESP"))* -- ESPE ETAPA
		     	(IFNULL(MLIN."U_EXP_GCM33",1)*:Factor)* -- material
		     	(FO0."U_EXP_QTT") --CantidadBase
				*CASE WHEN(:Merma=1) THEN (1+IFNULL(FLIN."U_EXP_MERMA",0)/100) ELSE 1 END "KG"		     	 
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"
    		 LEFT  JOIN "@EXP_FRSELA" "FLIN" ON FLIN."Code"=FO0."Code"   and FLIN."U_EXP_CODRUT"=RUT."LineId"
		     LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FLIN."U_EXP_MTRL"= MLIN."Name"
		     WHERE "ItemCode"=:ItemCode AND FLIN."U_EXP_MPRIMA" IS NOT NULL AND RUT."U_EXP_CODRUT" IN ('SEL')  
		     and  RUT."LineId" IN(select * from  "EXP_GetRutasKGDef"(:ItemCode,:IsRuta))     
			UNION --EXTRUSION DE LAMINA
			SELECT DISTINCT		
		     	:AnchoPT*--ANCHO PT
		     	:LargoPT*--LARGO PT
		     	(FLIN."U_EXP_ESPESOR"*"EXP_GetConversion"(FLIN."U_EXP_UNESP"))* -- ESPE ETAPA
		     	(IFNULL(MLIN."U_EXP_GCM33",1)*:Factor)* -- material
		     	(FO0."U_EXP_QTT") --CantidadBase
				*CASE WHEN(:Merma=1) THEN (1+IFNULL(FLIN."U_EXP_MERMA",0)/100) ELSE 1 END "KG"		     	 
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"
    		 LEFT  JOIN "@EXP_FREXTR" "FLIN" ON FLIN."Code"=FO0."Code"  and FLIN."U_EXP_CODRUTDE"=RUT."LineId"
		     LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FLIN."U_EXP_MATE"= MLIN."Name"
		     WHERE "ItemCode"=:ItemCode AND RUT."U_EXP_CODRUT" IN ('EXT')
		     and  RUT."LineId" IN(select * from  "EXP_GetRutasKGDef"(:ItemCode,:IsRuta))  	
			          		     
		     );


	     END IF;
		
		
	ELSEIF(:TipoForm='PK') THEN	-- PESO KILOS
 		IF(:Merma=1) THEN		
       SELECT IFNULL(SUM("A"),:KGCalculado)  INTO KGCalculado   
       FROM( SELECT "FACTOR"/ SUM("FACTOR") OVER (PARTITION BY "AUX" ) *:KGCalculado *"MERMA"  "A","RUT","EXT"
    
     FROM(
     	SELECT DISTINCT		
			1 "AUX",
			RUT."LineId" "RUT",
			FEXTR."U_EXP_CODRUTDE" "EXT",
			(IFNULL(FEXTR."U_EXP_ESPESOR",FLIN."U_EXP_ESPESOR")*"EXP_GetConversion"(IFNULL(FEXTR."U_EXP_UNESP",FLIN."U_EXP_UNESP")))* 
		     	(IFNULL(IFNULL(MLEX."U_EXP_GCM33",MLIN."U_EXP_GCM33"),1)*:Factor) "FACTOR",
			(1+IFNULL(IFNULL(FEXTR."U_EXP_MERMA",FLIN."U_EXP_MERMA"),0)/100) "MERMA"
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"		     
    		 LEFT  JOIN "@EXP_FRREBO" "FLIN" ON FLIN."Code"=FO0."Code"   and FLIN."U_EXP_CODRUT"=RUT."LineId"
			 LEFT JOIN "@EXP_FREXTR" "FEXTR" ON FEXTR."Code"=FO0."Code" AND FLIN."U_EXP_CODRUT" =FEXTR."U_EXP_CDRTDE"
		     LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FLIN."U_EXP_MTRL"= MLIN."Name"
		     LEFT JOIN "@EXP_TIPMAT" "MLEX" ON FEXTR."U_EXP_MATE"= MLEX."Name"		     
		     WHERE "ItemCode"=:ItemCode AND ((FLIN."U_EXP_MPRIMA" IS NOT NULL) OR FEXTR."U_EXP_CDRTDE" IS NOT NULL) AND RUT."U_EXP_CODRUT" IN ('REB') 
		     UNION ALL
     	SELECT DISTINCT		
			1 "AUX",
			RUT."LineId" "RUT",
			FEXTR."U_EXP_CODRUTDE" "EXT",
			(IFNULL(FEXTR."U_EXP_ESPESOR",FLIN."U_EXP_ESPESOR")*"EXP_GetConversion"(IFNULL(FEXTR."U_EXP_UNESP",FLIN."U_EXP_UNESP")))* 
		     	(IFNULL(IFNULL(MLEX."U_EXP_GCM33",MLIN."U_EXP_GCM33"),1)*:Factor) "FACTOR",
			(1+IFNULL(IFNULL(FEXTR."U_EXP_MERMA",FLIN."U_EXP_MERMA"),0)/100) "MERMA"
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"		     
    		 LEFT  JOIN "@EXP_FRHABI" "FLIN" ON FLIN."Code"=FO0."Code"   and FLIN."U_EXP_CODRUT"=RUT."LineId"
			 LEFT JOIN "@EXP_FREXTR" "FEXTR" ON FEXTR."Code"=FO0."Code" AND FLIN."U_EXP_CODRUT" =FEXTR."U_EXP_CDRTDE"
		     LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FLIN."U_EXP_MTRL"= MLIN."Name"
		     LEFT JOIN "@EXP_TIPMAT" "MLEX" ON FEXTR."U_EXP_MATE"= MLEX."Name"		     
		     WHERE "ItemCode"=:ItemCode AND ((FLIN."U_EXP_MPRIMA" IS NOT NULL) OR FEXTR."U_EXP_CDRTDE" IS NOT NULL) AND RUT."U_EXP_CODRUT" IN ('HAB') 
		     UNION ALL
     	SELECT DISTINCT		
			1 "AUX",
			RUT."LineId" "RUT",
			FEXTR."U_EXP_CODRUTDE" "EXT",
			(IFNULL(FEXTR."U_EXP_ESPESOR",FLIN."U_EXP_ESPESOR")*"EXP_GetConversion"(IFNULL(FEXTR."U_EXP_UNESP",FLIN."U_EXP_UNESP")))* 
		     	(IFNULL(IFNULL(MLEX."U_EXP_GCM33",MLIN."U_EXP_GCM33"),1)*:Factor) "FACTOR",
			(1+IFNULL(IFNULL(FEXTR."U_EXP_MERMA",FLIN."U_EXP_MERMA"),0)/100) "MERMA"
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"		     
    		 LEFT  JOIN "@EXP_FRCORT" "FLIN" ON FLIN."Code"=FO0."Code"   and FLIN."U_EXP_CODRUT"=RUT."LineId"
			 LEFT JOIN "@EXP_FREXTR" "FEXTR" ON FEXTR."Code"=FO0."Code" AND FLIN."U_EXP_CODRUT" =FEXTR."U_EXP_CDRTDE"
		     LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FLIN."U_EXP_MTRL"= MLIN."Name"
		     LEFT JOIN "@EXP_TIPMAT" "MLEX" ON FEXTR."U_EXP_MATE"= MLEX."Name"		     
		     WHERE "ItemCode"=:ItemCode AND ((FLIN."U_EXP_MPRIMA" IS NOT NULL) OR FEXTR."U_EXP_CDRTDE" IS NOT NULL) AND RUT."U_EXP_CODRUT" IN ('COR') 
		     UNION ALL		     		     
     	SELECT DISTINCT		
			1 "AUX",
			RUT."LineId" "RUT",
			FEXTR."U_EXP_CODRUTDE" "EXT",
			(IFNULL(FEXTR."U_EXP_ESPESOR",FLIN."U_EXP_ESPESOR")*"EXP_GetConversion"(IFNULL(FEXTR."U_EXP_UNESP",FLIN."U_EXP_UNESP")))* 
		     	(IFNULL(IFNULL(MLEX."U_EXP_GCM33",MLIN."U_EXP_GCM33"),1)*:Factor) "FACTOR",
			(1+IFNULL(IFNULL(FEXTR."U_EXP_MERMA",FLIN."U_EXP_MERMA"),0)/100) "MERMA"
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"		     
    		 LEFT  JOIN "@EXP_FRIMPR" "FLIN" ON FLIN."Code"=FO0."Code"   and FLIN."U_EXP_CODRUT"=RUT."LineId"
			 LEFT JOIN "@EXP_FREXTR" "FEXTR" ON FEXTR."Code"=FO0."Code" AND FLIN."U_EXP_CODRUT" =FEXTR."U_EXP_CDRTDE"
		     LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FLIN."U_EXP_MTRL"= MLIN."Name"
		     LEFT JOIN "@EXP_TIPMAT" "MLEX" ON FEXTR."U_EXP_MATE"= MLEX."Name"		     
		     WHERE "ItemCode"=:ItemCode AND ((FLIN."U_EXP_MPRIMA" IS NOT NULL) OR FEXTR."U_EXP_CDRTDE" IS NOT NULL) AND RUT."U_EXP_CODRUT" IN ('IMP') 
		     UNION ALL
		     
		    SELECT DISTINCT		
			1 "AUX",
			RUT."LineId" "RUT",
			FEXTR."U_EXP_CODRUTDE" "EXT",
			(IFNULL(FEXTR."U_EXP_ESPESOR",FLIN."U_EXP_ESPESOR")*"EXP_GetConversion"(IFNULL(FEXTR."U_EXP_UNESP",FLIN."U_EXP_UNESP")))* 
		     	(IFNULL(IFNULL(MLEX."U_EXP_GCM33",MLIN."U_EXP_GCM33"),1)*:Factor) "FACTOR",
			(1+IFNULL(IFNULL(FEXTR."U_EXP_MERMA",FLIN."U_EXP_MERMA"),0)/100) "MERMA"
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"		     
    		  JOIN "@EXP_LAMINA" "FLIN" ON FLIN."Code"=FO0."Code"  and FLIN."U_EXP_CODRUT"=RUT."LineId"
			 LEFT JOIN "@EXP_FREXTR" "FEXTR" ON FEXTR."Code"=FO0."Code" AND FLIN."U_EXP_CODRUT" =FEXTR."U_EXP_CDRTDE"
	     	 LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FLIN."U_EXP_MTRL"= MLIN."Name"			  
		     LEFT JOIN "@EXP_TIPMAT" "MLEX" ON FEXTR."U_EXP_MATE"= MLEX."Name"
		     WHERE "ItemCode"=:ItemCode AND ((FLIN."U_EXP_MPRIMA" IS NOT NULL) OR FEXTR."U_EXP_CDRTDE" IS NOT NULL) AND RUT."U_EXP_CODRUT" IN ('LAM') 
		     UNION ALL
		    SELECT DISTINCT		
			1 "AUX",
			RUT."LineId" "RUT",
			FEXTR."U_EXP_CODRUTDE" "EXT",
			(IFNULL(FEXTR."U_EXP_ESPESOR",FLIN."U_EXP_ESPESOR")*"EXP_GetConversion"(IFNULL(FEXTR."U_EXP_UNESP",FLIN."U_EXP_UNESP")))* 
		     	(IFNULL(IFNULL(MLEX."U_EXP_GCM33",MLIN."U_EXP_GCM33"),1)*:Factor) "FACTOR",
			(1+IFNULL(IFNULL(FEXTR."U_EXP_MERMA",FLIN."U_EXP_MERMA"),0)/100) "MERMA"
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"		     
    		  JOIN "@EXP_FRSELA" "FLIN" ON FLIN."Code"=FO0."Code"  and FLIN."U_EXP_CODRUT"=RUT."LineId"
			 LEFT JOIN "@EXP_FREXTR" "FEXTR" ON FEXTR."Code"=FO0."Code" AND FLIN."U_EXP_CODRUT" =FEXTR."U_EXP_CDRTDE"
	     	 LEFT JOIN "@EXP_TIPMAT" "MLIN" ON FLIN."U_EXP_MTRL"= MLIN."Name"			  
		     LEFT JOIN "@EXP_TIPMAT" "MLEX" ON FEXTR."U_EXP_MATE"= MLEX."Name"
		     WHERE "ItemCode"=:ItemCode AND ((FLIN."U_EXP_MPRIMA" IS NOT NULL) OR FEXTR."U_EXP_CDRTDE" IS NOT NULL) AND RUT."U_EXP_CODRUT" IN ('SEL') 
		     UNION ALL		     
  			SELECT DISTINCT		
			1 "AUX",
			RUT."LineId" "RUT",
			FEXTR."U_EXP_CODRUTDE" "EXT",
			(FEXTR."U_EXP_ESPESOR"*"EXP_GetConversion"(FEXTR."U_EXP_UNESP"))* 
		     	(IFNULL(MLEX."U_EXP_GCM33",1)*:Factor) "FACTOR",
			(1+IFNULL(FEXTR."U_EXP_MERMA",0)/100) "MERMA"
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"		   
			 LEFT JOIN "@EXP_FREXTR" "FEXTR" ON FEXTR."Code"=FO0."Code"  
		     LEFT JOIN "@EXP_TIPMAT" "MLEX" ON FEXTR."U_EXP_MATE"= MLEX."Name"
		     WHERE "ItemCode"=:ItemCode AND FEXTR."U_EXP_CDRTDE" IS  NULL AND RUT."U_EXP_CODRUT" IN ('EXT') 
		      
		     )) WHERE ("RUT"IN(select * from  "EXP_GetRutasKGDef"(:ItemCode,:IsRuta)) OR "EXT"IN(select * from  "EXP_GetRutasKGDef"(:ItemCode,:IsRuta)));

		     

		END IF;
	END IF;				
		     
		
 END;