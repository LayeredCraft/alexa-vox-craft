using AlexaVoxCraft.Model.Apl.Components;

namespace AlexaVoxCraft.Model.Apl.Tests.Components;

public class EditTextTests : TestBase<EditTextTests>
{
    [Fact]
    public async Task EditText_Serializes()
    {
        var control = new EditText
        {
            Hint = "Enter your name",
            KeyboardType = KeyboardType.Normal,
            SubmitKeyType = SubmitKeyType.Done,
            MaxLength = 50,
            SecureInput = false,
            SelectOnFocus = true
        };

        await TestHelper.VerifySerializedObject(control, AlexaJson, "EditText");
    }
}
