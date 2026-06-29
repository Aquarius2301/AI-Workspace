using Infrastructure.Functions.Summaries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers;

namespace WebApi.Controllers;

[ApiController]
[Route("api/summaries")]
[Authorize]
public class SummaryController : ControllerBase
{
    private readonly IMediator _mediator;

    public SummaryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get a summary overview for the Home page of the authenticated user.
    /// Includes team counts, project counts, task statistics, recent tasks, recent comments, and team summaries.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Returns the summary data</response>
    /// <response code="401">Unauthorized if the user is not authenticated</response>
    [HttpGet]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var userId = ClaimHelper.GetCurrentUserId();

        var summary = await _mediator.Send(
            new GetSummaryQuery(userId, cancellationToken),
            cancellationToken
        );

        return Ok(summary);
    }
}
