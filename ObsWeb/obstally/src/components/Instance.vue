<template lang="pug">
v-ons-page
    v-ons-toolbar
      ons-back-button Back
      .center {{instance.Name}}
      .right
        ons-toolbar-button(@click="settings($route.params.id)")
          //- router-link(:to="'/instance/'+this.$route.params.id+'/sources'")
          ons-icon(icon="fa-cog")
    .app
        .status
            span(v-show="instance.Online") ONLINE
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
.status {
  position: fixed;
  right: 10px;
  bottom: 10px;
}

.app {
  position: relative;
  // height 100%;
}
</style>