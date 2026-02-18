using MediatR;

namespace Discounts.Application.Features.Offers.Command.ApproveOffer
{
    public record ApproveOfferCommand(Guid OfferId) : IRequest<Unit>;
}
