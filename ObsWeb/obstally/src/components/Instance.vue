<template lang="pug">
v-ons-page
    v-ons-toolbar
      ons-back-button Back
      .center OBS Remote Tally
      .right
        ons-toolbar-button(@click="settings(id)")
          ons-icon(icon="fa-cog")
    .app
        span {{instance}}
        .status
            span(v-show="instance.Online") ONLINE
        Tally(:instance="instance")
</template>

<script>
import { db } from '../db'
import Sources from './Sources'

const instances = db.ref('instances');

export default {
  props:['id'],
    data() {
        return {
            instance: {},
    }
  },
  watch: {
    id: {
      // call it upon creation too
      immediate: true,
      handler(id) {
        this.$rtdbBind('instance', instances.child(id))
      },
    },
  },
  methods:{
    settings(id){
      console.log(id);
       this.$emit('push', {
        ...Sources, // Or 'extends: newPage'
        onsNavigatorProps: {
            id: id,
        }
        });
    }
  }
};
</script>

<style lang="stylus" scoped>
.status {
  position: fixed;
  right: 10px;
  bottom: 10px;
}

.app {
  position: relative;
}
</style>