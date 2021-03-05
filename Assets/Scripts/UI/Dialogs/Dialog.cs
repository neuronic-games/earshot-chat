namespace UI.Dialogs
{
    public static class Dialog
    {
        public static IDialogController Get()
        {
            return Locator<IDialogController, NullDialogController>.Get();
        }

        public static void Set(IDialogController set)
        {
            Locator<IDialogController, NullDialogController>.Set(set);
        }
    }
}