<template lang="pug">
 v-ons-page
    v-ons-toolbar
      .center OBS Remote Tally

    v-one-list
        ons-list-item(v-for="(instance,id) in instances" v-bind:key="id" tappable modifier="chevron" @click="push(id)")
            span {{instance.Name}} 
                span(v-show="instance.Pwd") LOCKED
</template>

<script>

import { db } from '../db'
import Instance from '../components/Instance'

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
        this.$emit('push', {
        ...Instance, // Or 'extends: newPage'
        onsNavigatorProps: {
            id: id,
        }
        });
    //   this.$emit('push', Instance,{id});
    }
  }
}
</script>

<style scoped>

</style>