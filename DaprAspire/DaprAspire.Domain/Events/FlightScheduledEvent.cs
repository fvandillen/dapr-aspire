namespace DaprAspire.Domain.Events;

public record FlightScheduledEvent(DateTime TimeScheduled, string AircraftType, string Destination);