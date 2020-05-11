<template lang="pug">
v-ons-page
    v-ons-toolbar.toolbar
      ons-back-button Back
      .center {{instance.Name}}
      .right
        ons-toolbar-button(@click="settings($route.params.id)")
          ons-icon(icon="fa-cog")
    .app
        .status
            span
              ons-icon(icon="fa-plug" :class="{'offline':!instance.Online}" class="icon")
        Tally(:instance="instance",:source="this.$route.params.source")
</template>

<script>
import { db } from '../db'
// import Sources from './Sources'

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
      handler() {
        this.$rtdbBind('instance', instances.child(this.$route.params.id))
      },
    },
  },
  methods:{
    settings(id){
        this.$router.push({path:'/instance/'+id+'/sources'});
    }
  }
};
</script>

<style lang="stylus" scoped>

.toolbar {
  position fixed
  top 0
  left 0
  right 0
}

.status {
  position: fixed;
  right: 15px;
  top: 70px;
}

.app {
  position: absolute;
  left 0
  right 0
  top 0
  bottom: 0
  z-index 10000
}

.icon{
  font-size 2em
  opacity 0.7
}

.offline
{
  opacity 0.3
}
</style>