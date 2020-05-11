<template lang="pug">
 v-ons-page
    v-ons-toolbar
      .left
        img(src="favicon.png")
      .center OBS Remote Tally
    
    v-ons-card
      .title Select which remote Open Broadcaster instance you want to receive notifications for.
      .content Your production engineer will need to be running the ObsBridge tool.

    v-one-list
        ons-list-item(v-for="(instance,id) in instances" v-bind:key="id" tappable modifier="chevron" @click="push(id)")
            //- ons-icon(icon="fa-lock" v-show="instance.Pwd" class="icon") 
            span {{instance.Name}}

</template>

<script>

import { db } from '../db'

export default {
    name: 'Instances',
    data() {
        return {
            instances: {},
    }
  },

  firebase: {
      instances: db.ref('instances'),
  },
  methods: {
    push(id) {
        this.$router.push({path:'/instance/'+id});
    }
  }
}
</script>

<style scoped>
.left img {
  height:60%;
  vertical-align: middle;
  margin-top: -3%;
  /* margin-left: 12%; */
  margin-left: .5em;;
  
}
</style>