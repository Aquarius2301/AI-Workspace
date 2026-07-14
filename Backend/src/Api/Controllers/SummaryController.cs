using AIWorkspace.Api.Helpers;
using AIWorkspace.Application.Features.Summaries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIWorkspace.Api.Controllers;

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
    /// Retrieves a summary overview for the current user's dashboard.
    /// </summary>
    /// <remarks>
    /// This action requires authentication. The current user's ID is extracted from the JWT claims.
    /// Returns an aggregated summary including:
    /// - Total number of teams the user belongs to
    /// - Total number of projects across those teams
    /// - Task summary (total, to-do, doing, done, overdue counts)
    /// - Recent 10 tasks assigned to the user
    /// - Summary of each team (name, role, member count, project count)
    /// </remarks>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <response code="200">Returns the dashboard summary (SummaryResponse).</response>
    /// <response code="401">User is not authenticated (Unauthorized).</response>
    /// <response code="500">An unexpected internal server error occurred (InternalServerError).</response>
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
