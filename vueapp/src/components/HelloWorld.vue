<script setup lang="ts">
    //import { defineComponent } from 'vue';
    import { Ref, ref, onMounted, watch } from 'vue'

    type Forecasts = {
        date: string
    }[];

    const loading: Ref<boolean> = ref(false)
    const post: Ref<null | Forecasts> = ref(null as null | Forecasts)

    onMounted(async () => {
        // fetch the data when the view is created and the data is
        // already being observed
        fetchData();
    })

    function fetchData() {
        post.value = null;
        loading.value = true;

        fetch('weatherforecast')
            .then(r => r.json())
            .then(json => {
                post.value = json as Forecasts;
                loading.value = false;
                return;
            });
    }

    watch(() => '$route', fetchData)
</script>

<template>
    <div class="post">
        <div v-if="loading" class="loading">
            Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationvue">https://aka.ms/jspsintegrationvue</a> for more details.
        </div>

        <div v-if="post" class="content">
            <table>
                <thead>
                    <tr>
                        <th>Date</th>
                        <th>Temp. (C)</th>
                        <th>Temp. (F)</th>
                        <th>Summary</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="forecast in post" :key="forecast.date">
                        <td>{{ forecast.date }}</td>
                        <td>{{ forecast.temperatureC }}</td>
                        <td>{{ forecast.temperatureF }}</td>
                        <td>{{ forecast.summary }}</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>
