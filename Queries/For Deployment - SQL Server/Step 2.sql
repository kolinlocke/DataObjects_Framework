/*
Note: Repeat this script until there are no more errors.
*/

/****** Object:  UserDefinedFunction [dbo].[udf_DataObjects_GetTableDef]    Script Date: 05/24/2012 17:33:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[udf_DataObjects_GetTableDef]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'Create Function [dbo].[udf_DataObjects_GetTableDef]
(@TableName VarChar(Max))	
Returns Table
As
Return
	(
	Select
		sCol.Column_id
		, sCol.Name As [ColumnName]
		, sTyp.Name As [DataType]
		, sCol.max_length As [Length]
		, sCol.Precision
		, sCol.Scale
		, sCol.Is_Identity As [IsIdentity]
		, Cast
		(
			(
			Case Count(IsCcu.Column_Name)
				When 0 Then 0
				Else 1
			End
			) 
		As Bit) As IsPk
	From 
		Sys.Columns As sCol
		Left Join Sys.Types As sTyp
			On sCol.system_type_id = sTyp.system_type_id
			And [sCol].User_Type_ID = [sTyp].User_Type_ID
		Inner Join Sys.Tables As sTab
			On sCol.Object_ID = sTab.Object_ID
		Inner Join Sys.Schemas As sSch
			On sSch.Schema_ID = sTab.Schema_ID
		Left Join Sys.Key_Constraints As Skc
			On sTab.Object_Id = Skc.Parent_Object_Id
			And Skc.Type = ''PK''
		Left Join INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE As IsCcu
			On Skc.Name = IsCcu.Constraint_Name
			And sTab.Name = IsCcu.Table_Name
			And sCol.Name = IsCcu.Column_Name
	Where
		sSch.Name + ''.'' + sTab.Name = @TableName
		And sCol.Is_Computed = 0
	Group By
		sCol.Name
		, sTyp.Name
		, sCol.max_length
		, sCol.Precision
		, sCol.Scale
		, sCol.Is_Identity
		, sCol.Column_id
	)


' 
END
GO
/****** Object:  Table [dbo].[DataObjects_Series]    Script Date: 05/24/2012 17:33:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataObjects_Series]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DataObjects_Series](
	[TableName] [varchar](1000) NULL,
	[LastID] [bigint] NULL
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DataObjects_Parameters]    Script Date: 05/24/2012 17:33:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataObjects_Parameters]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DataObjects_Parameters](
	[ParameterName] [varchar](50) NULL,
	[ParameterValue] [varchar](8000) NULL
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[usp_DataObjects_GetTableDef]    Script Date: 05/24/2012 17:33:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_DataObjects_GetTableDef]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE Procedure [dbo].[usp_DataObjects_GetTableDef]
@TableName VarChar(Max)
, @SchemaName VarChar(Max) = ''''
As
Set NOCOUNT On
Begin
	
	If IsNull(@SchemaName, '''') = ''''
	Begin
		Set @SchemaName = ''dbo''
	End
	
	Select *
	From [udf_DataObjects_GetTableDef](@SchemaName + ''.'' + @TableName)
	Order By Column_Id
	
End
' 
END
GO
/****** Object:  StoredProcedure [dbo].[usp_DataObjects_GetNextID]    Script Date: 05/24/2012 17:33:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_DataObjects_GetNextID]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'Create Procedure [dbo].[usp_DataObjects_GetNextID]
@TableName VarChar(Max)
As
Begin
	Declare @LastID BigInt
	Declare @Ct Int

	Select @Ct = Count(*)
	From DataObjects_Series
	Where TableName = @TableName
		
	If @Ct = 0
	Begin
		Insert Into DataObjects_Series (TableName, LastID) Values (@TableName, 0)
	End

	Select @LastID = LastID
	From DataObjects_Series
	Where TableName = @TableName
		
	Set @LastID = @LastID + 1
		
	Update DataObjects_Series
	Set LastID = @LastID 
	Where TableName = @TableName
	
	Select @LastID As [ID]
	
End

' 
END
GO
/****** Object:  StoredProcedure [dbo].[usp_DataObjects_Parameter_Set]    Script Date: 05/24/2012 17:33:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_DataObjects_Parameter_Set]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'Create Procedure [dbo].[usp_DataObjects_Parameter_Set]
@ParameterName VarChar(Max)
, @ParameterValue VarChar(Max)
As
Begin
	Declare @Ct As Int	
	Select @Ct = Count(1)
	From DataObjects_Parameters
	Where ParameterName = @ParameterName
	
	If @Ct = 0
	Begin
		Insert Into DataObjects_Parameters 
			(ParameterName, ParameterValue) 
		Values 
			(@ParameterName, @ParameterValue)
	End
	Else
	Begin
		Update DataObjects_Parameters 
		Set ParameterValue = @ParameterValue 
		Where ParameterName = @ParameterName
	End
End

' 
END
GO
/****** Object:  StoredProcedure [dbo].[usp_DataObjects_Parameter_Require]    Script Date: 05/24/2012 17:33:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_DataObjects_Parameter_Require]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'Create Procedure [dbo].[usp_DataObjects_Parameter_Require]
@ParameterName VarChar(Max)
, @ParameterValue VarChar(Max)
As
Begin
	Declare @Ct As Int	
	Select @Ct = Count(1)
	From DataObjects_Parameters
	Where ParameterName = @ParameterName
	
	If @Ct = 0
	Begin
		Insert Into DataObjects_Parameters 
			(ParameterName, ParameterValue) 
		Values 
			(@ParameterName, @ParameterValue)
	End
End

' 
END
GO
/****** Object:  StoredProcedure [dbo].[usp_DataObjects_Parameter_Get]    Script Date: 05/24/2012 17:33:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_DataObjects_Parameter_Get]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'Create Procedure [dbo].[usp_DataObjects_Parameter_Get]
@ParameterName VarChar(Max)
As
Begin
	Declare @ParameterValue As VarChar(Max)		
	Set @ParameterValue = ''''
	
	Declare @Ct As Int	
	Select @Ct = Count(1)
	From DataObjects_Parameters
	Where ParameterName = @ParameterName
	
	If @Ct = 0
	Begin
		Exec usp_DataObjects_Parameter_Require @ParameterName
	End
	Else
	Begin
		Select @ParameterValue = ParameterValue
		From DataObjects_Parameters
		Where ParameterName = @ParameterName
	End
	
	Select @ParameterValue As [ParameterValue]
End

' 
END
GO
