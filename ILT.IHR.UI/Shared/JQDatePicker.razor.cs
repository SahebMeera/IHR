#region Microsoft References
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

#region System References
using System;
using System.Threading.Tasks;
#endregion

namespace ILT.IHR.UI.Shared
{
    public partial class JQDatePicker
    {
        #region Parameter
        [Parameter]
        public DateTime? BindValue
        {
            get => _value;
            set
            {
                if (_value == value) return;

                _value = value;
                ValueChanged.InvokeAsync(BindValue);
            }
        }
        [Parameter] public string Format { get; set; }
        [Parameter] public EventCallback<DateTime?> ValueChanged { get; set; }
        [Parameter] public string Id { get; set; }
        [Parameter] public string Class { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public DateTime? MinDate { get; set; }
        [Parameter] public DateTime? MaxDate { get; set; }
        #endregion

        #region Inject
        [Inject] IJSRuntime JSRuntime { get; set; }
        #endregion
        private DateTime? _value;
        private string DatePickerFormat { get; set; }
        ElementReference currentElement { get; set; }

        #region Protected Method
        /// <summary>
        /// Method to initialize variable on component initialization
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            Format = string.IsNullOrEmpty(Format) ? "mm/dd/yy" : Format;
            DatePickerFormat = string.IsNullOrEmpty(DatePickerFormat) ?
                               "mm/dd/yy" : DatePickerFormat;
        }   
        /// <summary>
        ///
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await RenderDatePicker();
            }
            if (!firstRender)
                await SetMinMaxDate();
        }
        #endregion

        #region Private Method
        /// <summary>
        /// Method to invoke Date picker js function
        /// </summary>
        /// <returns></returns>
        private async Task RenderDatePicker()
        {
            await JSRuntime.InvokeVoidAsync("siteFunction.InitDatePickerwithSelect",
                            currentElement, DatePickerFormat, MinDate, MaxDate);
        }
        /// <summary>
        /// Method to invoke setminmaxdate js function
        /// </summary>
        /// <returns></returns>
        private async Task SetMinMaxDate()
        {
            await JSRuntime.InvokeVoidAsync("siteFunction.SetMinMaxDate",
                                             currentElement, MinDate, MaxDate);
        }
        /// <summary>
        /// Method to handle date picker change
        /// </summary>
        /// <param name="e">ChangeEventArgs</param>
        private void OnChange(ChangeEventArgs e)
        {
            DateTime dateTime;
            if (DateTime.TryParse(e.Value.ToString(), out dateTime))
            {
                ValueChanged.InvokeAsync(dateTime);
            }
        }
        private void OnInpuChange(ChangeEventArgs e)
        {
            DateTime dateTime;
            if (DateTime.TryParse(e.Value.ToString(), out dateTime))
            {
                ValueChanged.InvokeAsync(dateTime);
            }
        }

        
        #endregion
    }
}
