using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace  ILT.IHR.UI.Pages.DeleteConfirmation

{
    public class ConfirmBase: ComponentBase
    {
        protected bool ShowConfirmation { get; set; }

        protected bool disabled { get; set; } = false;

        [Parameter]
        public string ConfirmationTitle { get; set; } = "Confirm Delete";

        [Parameter]
        public string ConfirmationMessage { get; set; } = "Are you sure you want to delete";

        [Parameter]
        public string buttonType { get; set; } = "Delete";

        public void Show()
        {
            disabled = false;
            ShowConfirmation = true;
            StateHasChanged();
        }

        [Parameter]
        public EventCallback<bool> ConfirmationChanged { get; set; }

        protected async Task OnConfirmationChange(bool value)
        {
            if (disabled)
                return;
            disabled = true;
            ShowConfirmation = false;
            await ConfirmationChanged.InvokeAsync(value);
        }
    }
}