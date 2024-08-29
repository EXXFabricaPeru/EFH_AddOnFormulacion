CREATE FUNCTION "EXP_GetMedidas"
(  
 -- Add the parameters for the function here  
 IN ItemCode nvarchar(50)  
)  
returns result nvarchar(100) 

LANGUAGE SQLSCRIPT AS 
	ancho numeric(19,2);
	largo numeric(19,2);
	espesor numeric(19,2);
	UM_ancho nvarchar(10);
	UM_largo nvarchar(10);
	UM_espesor nvarchar(10);

BEGIN  
 
 	SELECT 
 		IFNULL(IT."U_EXP_PST",''),
 		IFNULL(IT."U_EXP_ANCHO",0),
 		IFNULL(IT."U_EXP_LARGO",0),
 		IFNULL(IT."U_EXP_ESP",0),
 		IFNULL(IT."U_EXP_UNDANCH",''),
 		IFNULL(IT."U_EXP_UNDLAR",''),
 		IFNULL(IT."U_EXP_UNDESP",'')
 	INTO result,ancho,largo, espesor,UM_ancho,UM_largo,UM_espesor
 	FROM OITM "IT" 
   --  LEFT JOIN "@EXP_CONMED" "UMAN" ON IT."U_EXP_UNDANCH"=UMAN."Code"
     --LEFT JOIN "@EXP_CONMED" "UMLA" ON IT."U_EXP_UNDLAR"=UMLA."Code"
     --LEFT JOIN "@EXP_CONMED" "UMES" ON IT."U_EXP_UNDESP"=UMES."Code"  
	WHERE IT."ItemCode"= :ItemCode;

if(result='') then
	select 
		CASE WHEN :ancho>0 THEN cast(:ancho as varchar)||:UM_ancho||' x 'ELSE '' END||
		CASE WHEN :largo>0 THEN :largo||:UM_largo||' x 'ELSE '' END||
		CASE WHEN :espesor>0 THEN :espesor||:UM_espesor||''ELSE '' END		
	into result 
	from dummy;

end if;

 -- Return the result of the function  
  
end;