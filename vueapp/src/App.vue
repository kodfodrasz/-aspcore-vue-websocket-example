<template>
    <img alt="Vue logo" src="./assets/logo.png">
    <HelloWorld msg="Welcome to Your Vue.js + TypeScript App" />
</template>

<style>
#app {
    font-family: Avenir, Helvetica, Arial, sans-serif;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    text-align: center;
    color: #2c3e50;
    margin-top: 60px; 
}
</style>

<script setup lang="ts">
import { Ref, ref, onMounted, onBeforeUnmount } from 'vue'
import { useWeatherStore } from "./stores/weather";

const weatherStore = useWeatherStore();
const websocket: Ref<WebSocket | null> = ref(null);

console.log(`VUE_APP_BASEURL: ${process.env.VUE_APP_BASEURL}`)

onMounted(async () => {
    wsConnect();
})
onBeforeUnmount(async () => {
    wsDisconnect()
})

function wsConnect() {
    const url = new URL('/api/ws', process.env.VUE_APP_BASEURL);
    url.protocol = url.protocol.replace('http', 'ws');
    var ws = new WebSocket(url);

    ws.onopen = (event) => {
        console.log("WebSocket connection opened:", event);
    };
    ws.onmessage = (event) => {
        console.log("WebSocket message received:", event.data);

        try {
            var forecasts = JSON.parse(event.data) as Forecasts
            if (forecasts) {
                weatherStore.update(forecasts)
            }
        } catch (error) {
            /* intentionally empty */
        }
    };
    ws.onerror = (error) => {
        console.log("WebSocket error:", error);
        weatherStore.disconnected();
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
    weatherStore.disconnected();
}
</script>

<script lang="ts">
import { defineComponent } from 'vue';
import HelloWorld from './components/HelloWorld.vue';

export default defineComponent({
    name: 'App',
    components: {
        HelloWorld
    }
});
</script>
