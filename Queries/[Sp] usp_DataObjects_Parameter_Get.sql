Create Procedure [dbo].[usp_DataObjects_Parameter_Get]
@ParameterName VarChar(Max)
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

GO


