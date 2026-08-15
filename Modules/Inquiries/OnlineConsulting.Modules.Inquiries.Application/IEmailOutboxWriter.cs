namespace OnlineConsulting.Modules.Inquiries.Application;

// Stages an outbox row on this module's own DbContext (via SharedKernel's EnqueueEmail extension)
// without saving - the Infrastructure implementation shares its DbContext instance with this
// module's repositories (both are scoped per-request), so whichever repository call happens next
// (e.g. IMessageRepository.AddAsync, which calls SaveChangesAsync internally) commits the staged
// outbox row(s) together with the business write, atomically, in that one call. Call Enqueue()
// BEFORE the repository call you want it bundled with - order matters here.
public interface IEmailOutboxWriter
{
    void Enqueue(string to, string subject, string htmlBody, string? cc = null, string? sourceReference = null);
}
