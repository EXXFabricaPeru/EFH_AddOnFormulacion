CREATE FUNCTION EXP_GetKGExtrusion(in ItemCode NVARCHAR(50), in Factor double, in Cantidad double, in ruta int) 
 RETURNS KGExtrusion Double 
 LANGUAGE SQLSCRIPT AS
 BEGIN
	Declare OnlyEXT  int;
	 Declare HaveLAM  int;
	 SELECT COUNT(*) INTO OnlyEXT	 FROM OITM "ITM" 
     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code" 
	  JOIN "@EXP_FFRUTA" "FRU" ON FO0."Code"=FRU."Code" 
	  WHERE  ITM."ItemCode"=:ItemCode AND FRU."U_EXP_CODRUT" NOT IN ('EXT') ;
	 SELECT COUNT(*) INTO HaveLAM	 
	 FROM OITM "ITM" 
     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code" 
	 JOIN "@EXP_FFRUTA" "FRU" ON FO0."Code"=FRU."Code" 
	 WHERE  ITM."ItemCode"=:ItemCode AND FRU."U_EXP_CODRUT" IN ('LAM') ;
	  
	  SELECT 
		     (CASE WHEN FO0."U_EXP_TIPO"='PK' THEN FO0."U_EXP_QTT" ELSE 0 END)
			 INTO KGExtrusion  
			 FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		      WHERE  ITM."ItemCode"=:ItemCode;	  
		      
IF(:KGExtrusion=0)THEN		     
	  
	  IF(:OnlyEXT>0)
	  THEN
	 SELECT 
		     SUM(CASE WHEN FO0."U_EXP_TIPO"='PK' THEN FO0."U_EXP_QTT" ELSE ROUND(
				(:Cantidad*1000) * 
				((IFNULL(ITM."U_EXP_LARGO",0) * (CASE WHEN IFNULL("U_EXP_FACLAR",0)=0 THEN 1 ELSE "U_EXP_FACLAR" END))+ (CASE WHEN IFNULL("U_EXP_REFLAR",0)=0 THEN 0 ELSE "U_EXP_REFLAR" END)*2)*
				IFNULL(CML."U_EXP_TOMET",0) * 
				((IFNULL(ITM."U_EXP_ANCHO",0) * (CASE WHEN IFNULL("U_EXP_FACANC",0)=0 THEN 1 ELSE "U_EXP_FACANC" END))+ (CASE WHEN IFNULL("U_EXP_REFANC",0)=0 THEN 0 ELSE "U_EXP_REFANC" END)*2)*
				IFNULL(CMA."U_EXP_TOMET",0)*
				IFNULL(FO1."U_EXP_ESPESOR",0)*  
				(CASE WHEN "CME"."Code" = 'Mils' then 0.0000254 else IFNULL(CME."U_EXP_TOMET",0) end)*
				IFNULL(MAT."U_EXP_GCM33",0)* 
				:Factor * 
				(1+IFNULL(FO1."U_EXP_MERMA",0)/100)) END)
			 INTO KGExtrusion  
			 FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		      JOIN "@EXP_FREXTR" "FO1" ON FO0."Code"=FO1."Code"
		     LEFT JOIN "@EXP_CONMED" "CML" ON ITM."U_EXP_UNDLAR"= CML."Code"
		     LEFT JOIN "@EXP_CONMED" "CMA" ON ITM."U_EXP_UNDANCH"= CMA."Code"
		     LEFT JOIN "@EXP_CONMED" "CME" ON FO1."U_EXP_UNESP"= CME."Code"
		     LEFT JOIN "@EXP_TIPMAT" "MAT" ON FO1."U_EXP_MATE"= MAT."Name"
		     WHERE "ItemCode"=:ItemCode AND FO1."U_EXP_CODRUTDE"=:ruta;  
 
	     ELSE
	     
	      IF(:HaveLAM>0)
	  THEN
		
     SELECT SUM("KG") 
     INTO KGExtrusion  
     FROM(
		     SELECT :Cantidad*
		     		:Factor*
		     		(ITM."U_EXP_ANCHO"+ITM."U_EXP_REFANC"*2)*
		     		CMA."U_EXP_TOMET"*
		     		IFNULL(MLAM."U_EXP_GCM33",1)*
		     		FLAM."U_EXP_ESPESOR" *
		     		"CMELAM"."U_EXP_TOMET"
		     		*(1+IFNULL(FLAM."U_EXP_MERMA",0)/100) "KG" 
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"
		     LEFT JOIN "@EXP_LAMINA" "FLAM" ON FLAM."Code"=FO0."Code"	   	      		      
		     LEFT JOIN "@EXP_CONMED" "CML" ON ITM."U_EXP_UNDLAR"= CML."Code"
		     LEFT JOIN "@EXP_CONMED" "CMA" ON ITM."U_EXP_UNDANCH"= CMA."Code"
		     LEFT JOIN "@EXP_CONMED" "CMELAM" ON FLAM."U_EXP_UNESP"= CMELAM."Code"
		     LEFT JOIN "@EXP_TIPMAT" "MLAM" ON FLAM."U_EXP_MTRL"= MLAM."Name"		     		     
		     WHERE "ItemCode"=:ItemCode AND RUT."U_EXP_CODRUT" IN ('LAM') AND RUT."LineId"=:ruta
		     UNION
		     		     SELECT :Cantidad*
		     		:Factor*
		     		(ITM."U_EXP_ANCHO"+ITM."U_EXP_REFANC"*2)*
		     		CMA."U_EXP_TOMET"*
		     		IFNULL(MIMP."U_EXP_GCM33",1)*
		     		FIMP."U_EXP_ESPESOR" *
		     		"CMEIMP"."U_EXP_TOMET"
		     		*(1+IFNULL(FIMP."U_EXP_MERMA",0)/100) "KG"
		     FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		     JOIN "@EXP_FFRUTA" "RUT"ON   RUT."Code"=FO0."Code"
    		 LEFT  JOIN "@EXP_FRIMPR" "FIMP" ON FIMP."Code"=FO0."Code"  
		     LEFT JOIN "@EXP_CONMED" "CML" ON ITM."U_EXP_UNDLAR"= CML."Code"
		     LEFT JOIN "@EXP_CONMED" "CMA" ON ITM."U_EXP_UNDANCH"= CMA."Code"
		     LEFT JOIN "@EXP_CONMED" "CMEIMP" ON FIMP."U_EXP_UNESP"= CMEIMP."Code"
		     LEFT JOIN "@EXP_TIPMAT" "MIMP" ON FIMP."U_EXP_MTRL"= MIMP."Name"
		     WHERE "ItemCode"=:ItemCode AND RUT."U_EXP_CODRUT" IN ('IMP') AND RUT."LineId"=:ruta
		     );

		     	  ELSE
		     	  
		     	  	     SELECT 	     
		     SUM(
		     :Factor*1000*(
				(FO0."U_EXP_QTT") * 
				IFNULL(FLAM."U_EXP_ESPESOR",1)*
				IFNULL(CASE WHEN CMEEXT."Code" = 'Mils' then 0.0000254 else IFNULL(CMEEXT."U_EXP_TOMET",0) end,1)*
				IFNULL(MEXT."U_EXP_GCM33",1)/
				(:Factor*1000*
				(IFNULL(FLAM."U_EXP_ESPESOR",1)*IFNULL(CASE WHEN CMELAM."Code" = 'Mils' then 0.0000254 else IFNULL(CMELAM."U_EXP_TOMET",0) end,1)*IFNULL(MLAM."U_EXP_GCM33",1))+
				(IFNULL(FIMP."U_EXP_ESPESOR",1)*IFNULL(MIMP."U_EXP_GCM33",1))
				)*
				(1+IFNULL(FO1."U_EXP_MERMA",0)/100))) 
			 INTO KGExtrusion  
			 FROM OITM "ITM" 
		     JOIN "@EXP_OFRM" "FO0" ON ITM."U_EXP_FORM"=FO0."Code"
		      JOIN "@EXP_FREXTR" "FO1" ON FO0."Code"=FO1."Code"
    		 LEFT  JOIN "@EXP_FRIMPR" "FIMP" ON FIMP."Code"=FO1."Code"  and  FO1."U_EXP_CDRTDE"=	FIMP."U_EXP_CODRUT"	     
		     LEFT JOIN "@EXP_LAMINA" "FLAM" ON FLAM."Code"=FO1."Code"	 and  FO1."U_EXP_CDRTDE"=	FLAM."U_EXP_CODRUT"	     	      		      
		     LEFT JOIN "@EXP_CONMED" "CML" ON ITM."U_EXP_UNDLAR"= CML."Code"
		     LEFT JOIN "@EXP_CONMED" "CMA" ON ITM."U_EXP_UNDANCH"= CMA."Code"
		     LEFT JOIN "@EXP_CONMED" "CMEEXT" ON FO1."U_EXP_UNESP"= CMEEXT."Code"
		     LEFT JOIN "@EXP_CONMED" "CMELAM" ON FLAM."U_EXP_UNESP"= CMELAM."Code"
		     LEFT JOIN "@EXP_TIPMAT" "MEXT" ON FO1."U_EXP_MATE"= MEXT."Name"
		     LEFT JOIN "@EXP_TIPMAT" "MIMP" ON FIMP."U_EXP_MTRL"= MIMP."Name"
		     LEFT JOIN "@EXP_TIPMAT" "MLAM" ON FLAM."U_EXP_MTRL"= MLAM."Name"		     		     
		     WHERE "ItemCode"=:ItemCode; 
	     END IF;
     
END IF;
END IF;

 END;


