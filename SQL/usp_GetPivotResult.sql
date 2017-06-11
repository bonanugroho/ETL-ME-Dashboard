USE [ETLManageEngine]
GO

/****** Object:  StoredProcedure [NetworkUtilization].[usp_GetPivotResult]    Script Date: 12/06/2017 1:45:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Bona Nugroho
-- Create date: 25 Feb 2015
-- Description:	Pivot Result
-- =============================================
CREATE PROCEDURE  [NetworkUtilization].[usp_GetPivotResult]
	-- Add the parameters for the stored procedure here
	@startDate date, 
	@endDate date
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	DECLARE @perfTable TABLE(router_id bigint, router_displayname nvarchar(250), regional nvarchar(100), induk nvarchar(100), interface_id bigint, 
		tunnel_name nvarchar(100), link_type nvarchar(5), link_provider nvarchar(50), speed int, calYear int, calMonth int, MaximumValue bigint,
		persen decimal(10,2), sDate Date, Ranking bigint);


	INSERT INTO @perfTable
	EXEC	[NetworkUtilization].[usp_GetRawFactUtil]
			@startDate = @startDate,
			@endDate = @endDate


	--select * from @perfTable

	select
		tblMaster.router_id
		, tblMaster.regional
		, tblMaster.induk
		, tblMaster.router_displayname
		, tblMaster.sDate
		, resProvider.[1] as Provider1
		, resLinkType.[1] as LinkType1
		, resSpeed.[1] as Speed1
		, resPersen.[1] as Persen1
		, resProvider.[2] as Provider2
		, resLinkType.[2] as LinkType2
		, resSpeed.[2] as Speed2
		, resPersen.[2] as Persen2
	from 
	(
		select
			distinct
			a.router_id
			, a.router_displayname
			, a.regional
			, a.induk
			, a.sDate
		from @perfTable a
	) as tblMaster
	inner join 
	(
		select
		* 
		from
		(
			select
				router_id
				, router_displayname
				, Ranking
				, link_provider
			from @perfTable
		) tblProvider
		pivot
		(
			max(link_provider)
			FOR Ranking IN ([1],[2]) 
		) pvtProvider
	) resProvider on tblMaster.router_id = resProvider.router_id
	inner join
	(
		select
		* 
		from
		(
			select
				router_id
				, router_displayname
				, Ranking
				, link_type
			from @perfTable
		) tblLinkType
		pivot
		(
			max(link_type)
			FOR Ranking IN ([1],[2]) 
		) pvtLinkType
	) resLinkType on tblMaster.router_id = resLinkType.router_id
	inner join
	(
		select
		* 
		from
		(
			select
				router_id
				, router_displayname
				, Ranking
				, speed
			from @perfTable
		) tblSpeed
		pivot
		(
			max(speed)
			FOR Ranking IN ([1],[2]) 
		) pvtSpeed
	) resSpeed on tblMaster.router_id = resSpeed.router_id
	inner join
	(
		select
		* 
		from
		(
			select
				router_id
				, router_displayname
				, Ranking
				, persen
			from @perfTable
		) tblPersen
		pivot
		(
			max(persen)
			FOR Ranking IN ([1],[2]) 
		) pvtPersen
	) resPersen on tblMaster.router_id = resPersen.router_id




END

GO


