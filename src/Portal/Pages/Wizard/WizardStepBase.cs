using Microsoft.AspNetCore.Components;
using PartnerAppPortal.Core.Models.Assessments;
using PartnerAppPortal.Portal.Services;

namespace PartnerAppPortal.Portal.Pages.Wizard
{
    public abstract class WizardStepBase : ComponentBase
    {
        [Inject] public IAssessmentApiClient Api { get; set; } = default!;
        [Inject] public WizardState State { get; set; } = default!;

        [Parameter] public Guid AssessmentId { get; set; }
        [Parameter] public EventCallback OnNext { get; set; }
        [Parameter] public EventCallback OnBack { get; set; }

        protected IReadOnlyList<AssessmentQuestion>? questions;
        protected string Title = "";

        /// <summary>Legacy section key; category question steps override with Category.</summary>
        protected abstract string Section { get; }

        protected override void OnParametersSet()
        {
            if (AssessmentId != Guid.Empty)
                State.AssessmentId = AssessmentId;
        }

        protected void SetAnswer(string questionId, string value)
        {
            State.SetAnswer(Section, questionId, value);
        }

        protected string? GetAnswer(string questionId)
        {
            return State.GetAnswer(Section, questionId);
        }
    }
}
