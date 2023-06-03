export { };

declare global {
  type ForecastSummary =
    | "Freezing"
    | "Bracing"
    | "Chilly"
    | "Cool"
    | "Mild"
    | "Warm"
    | "Balmy"
    | "Hot"
    | "Sweltering"
    | "Scorching"

  type Forecast = {
    date: string,
    temperatureC: number,
    temperatureF: number,
    summary: ForecastSummary | null,
  }
  type Forecasts = Forecast[];
}