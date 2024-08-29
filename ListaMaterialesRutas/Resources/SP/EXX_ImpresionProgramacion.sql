CREATE PROCEDURE "EXX_ImpresionProgramacion"
(	IN FINI DATETIME,
	IN FFIN DATETIME,
	IN HORAI NVARCHAR(12),
	IN HORAF NVARCHAR(12),
	IN Etapa INT,
	IN RECURSO NVARCHAR(50)
)
AS
BEGIN

	declare FiltroFecha int;
	declare FiltroOrden int;
	declare FiltroRecurso int;
	declare FiltroHora int;
	
	IF(RECURSO <> '') THEN
		FiltroRecurso := 1;
	ELSE
		FiltroRecurso := 0;
	END IF;

	IF(FINI = '' AND FFIN = '') THEN
		FiltroFecha := 0 ;
	END IF;
	
	IF(FINI <> '' AND FFIN <> '') THEN
		FiltroFecha := 1;
	END IF;

	IF(FINI = '' AND FFIN <> '') THEN 
		FiltroFecha := 2 ;
	END IF;

	IF(FINI <> '' AND FFIN = '') THEN
		FiltroFecha := 3 ;
	END IF;
	

	
	select T0."DocEntry" "OrderDE",T0."DocNum" "Order",T1."ItemCode" "ResCode",T2."ResName" "Resource",
			TO_VARCHAR(TO_TIME(ADD_SECONDS (TO_TIMESTAMP ('2021-01-01 00:00:00'), t1."BaseQty"*t2."TimeResUn"*t0."PlannedQty"))) "Hours",
			T4."StartDate",T4."EndDate" "FinishDate",IFNULL(T1."U_EXC_Programado",'N') "Scheduled", T0."Priority" "Priority",
			T0."DueDate" "DelivDate",T3."CardCode",
			T3."CardName" "Customer",T0."ItemCode",TM."ItemName" "Product",
			T0."Uom" "UOM",T0."PlannedQty" "ReqQuant",
			CASE T1."U_EXC_Programado" WHEN 'N' THEN '00:00' ELSE LEFT(RIGHT('000'||TO_VARCHAR("U_EXC_HoraIni"),4),2)||':'||RIGHT('0'||TO_VARCHAR("U_EXC_HoraIni"),2) END "StartTime",
			CASE T1."U_EXC_Programado" WHEN 'N' THEN '00:00' ELSE LEFT(RIGHT('000'||TO_VARCHAR("U_EXC_HoraFin"),4),2)||':'||RIGHT('0'||TO_VARCHAR("U_EXC_HoraFin"),2) END "FinishTime",
			T1."PlannedQty",T1."VisOrder" "OLineNum",T2."TimeResUn",T1."StageId",t4."StgEntry",
			CASE IFNULL(T1."U_EXC_Programado", 'N') WHEN 'N' THEN NULL ELSE "U_EXC_FProgam" END "ProgramDate",
			CASE IFNULL(T1."U_EXC_Programado", 'N') WHEN 'N' THEN NULL ELSE T1."U_EXC_FPROGF" END  "ProgramDateEnd",
			T1."U_EXX_DSTANDBY" "Standby",
			T1."U_EXX_DPARCIAL" "Parcial",
			T1."U_EXX_DTERMIN" "Terminado",
			(Select O."Desc" from ORST O Where O."AbsEntry" = T4."StgEntry") "Etapa"
			from OWOR T0
			INNER JOIN WOR1 T1 ON T0."DocEntry"=T1."DocEntry"
			INNER JOIN WOR4 T4 ON T0."DocEntry"=T4."DocEntry" and T4."StageId"=T1."StageId"
			INNER JOIN OITM TM ON T0."ItemCode"=TM."ItemCode"
			LEFT JOIN ORSC T2 ON T1."ItemCode"=T2."ResCode"
			LEFT JOIN OCRD T3 ON T0."CardCode"=T3."CardCode"
			WHERE T0."Status"='R' AND T1."ItemType"=290
			AND ((:FiltroRecurso = 1 AND T1."ItemCode" = :RECURSO AND T4."StgEntry"=:ETAPA) OR :FiltroRecurso = 0) AND
			((:FiltroFecha = 3 AND T0."DueDate" >= :FINI) OR (:FiltroFecha = 2 AND T0."DueDate" <= :FFIN) OR (:FiltroFecha = 1 AND T0."DueDate" between :FINI and :FFIN) OR (:FiltroFecha = 0));
			
	END;