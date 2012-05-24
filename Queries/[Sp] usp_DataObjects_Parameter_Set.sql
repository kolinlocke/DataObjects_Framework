Create Procedure [dbo].[usp_DataObjects_Parameter_Set]
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

GO


