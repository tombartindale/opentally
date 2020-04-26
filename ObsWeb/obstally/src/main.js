import Vue from 'vue'
import App from './App.vue'
import { rtdbPlugin } from 'vuefire'
import VueRouter from 'vue-router'
import Instances from  './components/Instances'
import Instance from  './components/Instance'
import Tally from  './components/Tally'
import Sources from  './components/Sources'
import VueOnsen from 'vue-onsenui'

import 'onsenui/css/onsenui.css';
import 'onsenui/css/onsen-css-components.css';


Vue.use(VueRouter)
Vue.use(VueOnsen)
Vue.config.productionTip = false
/* eslint-disable */

const router = new VueRouter({
    routes: [
        { path: '/',name:'Home', component: Instances, children:[
            { path: '/instance/:id/:source?', name:'Instance', component: Instance, children:[
                { path: '/instance/:id/sources',name:'Sources', component: Sources }
            ] }
        ]},
    ]
})

Vue.use(rtdbPlugin)

Vue.component('Tally',Tally);

var appSettings = {
    selectedTally:'NONE'
}

new Vue({
    router,
    data() {
        return {
          appSettings
        };
      },
  render: h => h(App)
}).$mount('#app')
