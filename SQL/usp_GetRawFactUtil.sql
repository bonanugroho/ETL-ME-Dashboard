USE [ETLManageEngine]
GO

/****** Object:  StoredProcedure [NetworkUtilization].[usp_GetRawFactUtil]    Script Date: 12/06/2017 1:45:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Bona Nugroho
-- Create date: 25 Feb 2015
-- Description:	Mengambil data raw Network Utilization
-- =============================================
CREATE PROCEDURE [NetworkUtilization].[usp_GetRawFactUtil]
	-- Add the parameters for the stored procedure here
	@startDate date, 
	@endDate date
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	DECLARE @perfTable TABLE(interface_id bigint, calYear int, calMonth int, MaximumValue bigint );


	INSERT INTO @perfTable
		SELECT
			tblOne.interface_id
			, tblOne.calYear
			, tblOne.calMonth
			, max(tblOne.AvgMax) as MaximumValue
		FROM
		(
			SELECT [interface_id]
				  ,DATEPART(YEAR, [FullDate]) calYear
				  ,DATEPART(MONTH, [FullDate]) calMonth
				  ,[factSource]
				  ,Avg([factMaxOctets]) as AvgMax
			FROM [NetworkUtilization].[factUtilization]
			WHERE FullDate BETWEEN @startDate AND @endDate
			--WHERE FullDate BETWEEN '2015-02-01' AND '2015-02-28'
			AND DATEPART(HOUR, [FullDate]) between 6 and 18 -- jam 6 s/d 18
			AND DATEPART(WEEKDAY, [FullDate]) between 2 and 6 -- Hari Senin s/d Jumat

			GROUP BY
				[interface_id]
				,DATEPART(YEAR, [FullDate]) 
				,DATEPART(MONTH, [FullDate])
				,[factSource]
		) as tblOne
		GROUP BY
			tblOne.interface_id
			, tblOne.calYear
			, tblOne.calMonth


	select
		*
		, CONVERT(date,CONVERT(varchar(4), tbl.calYear) + '-' + CONVERT(varchar(2),tbl.calMonth) + '-01') as sDate
		, DENSE_RANK() OVER 
		(PARTITION BY tbl.router_id ORDER BY tbl.persen desc) AS Ranking

	from
	(
		SELECT 
			ocbg.CabangId as router_id
			, ocbg.DisplayName as router_displayname
			, ocbg.Regional
			, ocbg.Induk
			, oIntf.InterfaceId as interface_id
			, oIntf.TunnelName as tunnel_name
			, oIntf.Link_Type as link_type
			, oIntf.Link_Provider as link_provider
			, oIntf.Bandwidth as speed
			, ofact.calYear
			, ofact.calMonth
			, ofact.MaximumValue
			, ((convert(decimal, ofact.MaximumValue) / 8) / convert(decimal, oIntf.Bandwidth) * 100)  as persen
		FROM @perfTable ofact
		INNER JOIN [NetworkUtilization].[dimInterfaces] oIntf
		ON ofact.[interface_id] = oIntf.InterfaceId
		INNER JOIN [NetworkUtilization].[dimCabang] ocbg
		ON oIntf.CabangId = ocbg.CabangId

			
		
	) as tbl



END



GO


