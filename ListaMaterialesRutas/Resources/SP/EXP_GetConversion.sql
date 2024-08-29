CREATE FUNCTION "EXP_GetConversion"
(  
	IN UM nvarchar(50)  
)  
returns result DOUBLE 

LANGUAGE SQLSCRIPT AS 
BEGIN  
	

	
	SELECT TOP 1 "UM" INTO result 
	FROM
	(SELECT 1 "UM",'∆∆' "Code" FROM DUMMY 
	UNION ALL
	SELECT 
		IFNULL(CASE WHEN CM."Code" = 'Mils' then 0.0000254 else IFNULL(CM."U_EXP_TOMET",1) end,1)"UM" ,CM."Code"		
	
	FROM "@EXP_CONMED" "CM"
	WHERE CM."Code"=:UM
	ORDER BY "Code" ASC
	);
  
end;