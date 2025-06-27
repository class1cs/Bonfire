namespace Bonfire.Domain.Entities;

public class FriendRequest
{
    public long Id { get; set; }
    
    public long SenderId { get; set; }
    
    public User Sender { get; set; }

    public long ReceiverId { get; set; }
    
    public User Receiver { get; set; }
    
    public FriendRequestStatus Status { get; set; } = FriendRequestStatus.Pending;
}

public enum FriendRequestStatus
{
    Pending = 0,

    Accepted = 1,

    Declined = 2,
}