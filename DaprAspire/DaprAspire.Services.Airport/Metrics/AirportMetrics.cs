using System.Diagnostics.Metrics;

namespace DaprAspire.Services.Airport.Metrics;

public class AirportMetrics
{
	private static Counter<int>? _flightsScheduled;
	
	public AirportMetrics()
	{
		var meter = new Meter("DaprAspire.Services.Airport");
		_flightsScheduled = meter.CreateCounter<int>("flights.scheduled");
	}
	
	public void FlightScheduled()
	{
		_flightsScheduled?.Add(1);
	}
}