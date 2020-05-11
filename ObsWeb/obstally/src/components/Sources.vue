<template lang="pug">
 v-ons-page
    v-ons-toolbar
        ons-back-button Done
        .center Select Source

    v-one-list
        ons-list-item(@click="select('')" tappable modifier="chevron")
            span Program Out
        ons-list-item(v-for="(source,index) in sources" v-bind:key="index" tappable modifier="chevron" @click="select(source)")
            span {{source}} 
</template>

<script>

import { db } from '../db'

const instances = db.ref('instances');

export default {
    props:['id'],
    name: 'Sources',
    data() {
        return {
            sources: {},
    }
  },
  watch: {
    id: {
      // call it upon creation too
      immediate: true,
      handler() {
        this.$rtdbBind('sources', instances.child(this.$route.params.id).child('Sources'))
      },
    },
  },
  methods: {
    select(source) {
      this.$router.push({path:'/instance/'+this.$route.params.id+'/'+source})
    }
  }
}
</script>