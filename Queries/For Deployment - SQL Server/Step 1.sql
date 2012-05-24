/****** Object:  StoredProcedure [dbo].[usp_DataObjects_Parameter_Get]    Script Date: 05/24/2012 17:34:07 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_DataObjects_Parameter_Get]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_DataObjects_Parameter_Get]
GO
/****** Object:  StoredProcedure [dbo].[usp_DataObjects_Parameter_Require]    Script Date: 05/24/2012 17:34:07 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_DataObjects_Parameter_Require]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_DataObjects_Parameter_Require]
GO
/****** Object:  StoredProcedure [dbo].[usp_DataObjects_Parameter_Set]    Script Date: 05/24/2012 17:34:07 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_DataObjects_Parameter_Set]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_DataObjects_Parameter_Set]
GO
/****** Object:  StoredProcedure [dbo].[usp_DataObjects_GetNextID]    Script Date: 05/24/2012 17:34:07 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_DataObjects_GetNextID]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_DataObjects_GetNextID]
GO
/****** Object:  StoredProcedure [dbo].[usp_DataObjects_GetTableDef]    Script Date: 05/24/2012 17:34:07 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_DataObjects_GetTableDef]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_DataObjects_GetTableDef]
GO
/****** Object:  Table [dbo].[DataObjects_Parameters]    Script Date: 05/24/2012 17:34:07 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataObjects_Parameters]') AND type in (N'U'))
DROP TABLE [dbo].[DataObjects_Parameters]
GO
/****** Object:  Table [dbo].[DataObjects_Series]    Script Date: 05/24/2012 17:34:07 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataObjects_Series]') AND type in (N'U'))
DROP TABLE [dbo].[DataObjects_Series]
GO
/****** Object:  UserDefinedFunction [dbo].[udf_DataObjects_GetTableDef]    Script Date: 05/24/2012 17:34:07 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[udf_DataObjects_GetTableDef]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[udf_DataObjects_GetTableDef]
GO
