<script setup lang="ts">
    //import { defineComponent } from 'vue';
    import { Ref, ref, onMounted, onBeforeUnmount } from 'vue'

    type Forecasts = {
        date: string
    }[];

    const loading: Ref<boolean> = ref(false)
    const post: Ref<null | Forecasts> = ref(null as null | Forecasts)
    const websocket: Ref<WebSocket | null> = ref(null);

    onMounted(async () => {
        // fetch the data when the view is created and the data is
        // already being observed
        wsConnect();
    })
    onBeforeUnmount(async () => {
        wsDisconnect()
    })

    function wsConnect() {
        post.value = null;
        loading.value = true;

        var ws = new WebSocket('ws://localhost:5043/ws');

        ws.onopen = (event) => {
            console.log("WebSocket connection opened:", event);
        };
        ws.onmessage = (event) => {
            console.log("WebSocket message received:", event.data);

            try {
                var forecasts = JSON.parse(event.data) as Forecasts
                if (forecasts) {
                    post.value = forecasts;
                    loading.value = false;
                }
            } catch (error) {
                /* intentionally empty */
            }
        };
        ws.onerror = (error) => {
            console.log("WebSocket error:", error);
        };

        const ping = setInterval(() => {
            const msg = `ping ${Date.now()}`;
            ws.send(msg)
            console.log("WebSocket message sent:", msg);
        }, 60000);

        ws.onclose = (event) => {
            clearTimeout(ping);
            console.log("WebSocket connection closed:", event.code);
        };

        websocket.value = ws;
    }

    function wsDisconnect() {
        websocket.value?.close(1000, "Goodbye");
        websocket.value = null;
    }
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
