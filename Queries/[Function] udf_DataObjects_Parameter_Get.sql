Create Function [dbo].[udf_DataObjects_Parameter_Get]
(@ParameterName VarChar(Max))
Returns VarChar(Max)
As
Begin
	Declare @ParameterValue As VarChar(Max)		
	Set @ParameterValue = ''
	
	Declare @Ct As Int	
	Select @Ct = Count(1)
	From DataObjects_Parameters
	Where ParameterName = @ParameterName
	
	If @Ct = 0
	Begin
		Return ''
	End
	Else
	Begin
		Select @ParameterValue = ParameterValue
		From DataObjects_Parameters
		Where ParameterName = @ParameterName
	End
	
	Return @ParameterValue
End

GO


