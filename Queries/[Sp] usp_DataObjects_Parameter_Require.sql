Create Procedure [dbo].[usp_DataObjects_Parameter_Require]
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

GO


