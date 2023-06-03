import { defineStore } from 'pinia'

type ForecastStoreState = {
  forecasts: Forecasts | null
}

function getDefault(): ForecastStoreState {
  return {
    forecasts: null
  }
}

export const useWeatherStore = defineStore('weather', {
  state: getDefault,
  actions: {
    update(data: Forecasts) {
      this.forecasts = data
    },
    disconnected() {
      this.forecasts = null;
    }
  },
})