public interface IKeyboardSelectable
{
    bool IsKeyboardSelectable { get; }
    void SetKeyboardSelected(bool selected);
    void SubmitKeyboard();
}