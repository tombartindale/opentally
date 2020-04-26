<template lang="pug">
 v-ons-page
    v-ons-toolbar
      .center Select Source

    v-one-list
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
      handler(id) {
        this.$rtdbBind('sources', instances.child(id).child('Sources'))
      },
    },
  },
  methods: {
    select(source) {
      console.log(source);
      this.$root.$data.appSettings.selectedTally = source;
      this.$emit('pop');
    }
  }
}
</script>