<template lang="pug">
.tally(:class="{ live: liveMe, preview: previewMe }")
    .message
        span(v-show="previewMe && !liveMe") STANDBY
        span(v-show="liveMe") LIVE
    div(:class="{recording: liveOut}")
    .label
        .text {{tally}}
</template>

<script>
export default {
    name: 'Tally',
    props:['instance','source'],
    computed:{
        liveOut(){
            return this.instance.Streaming || this.instance.Recording;
        },
        
        liveMe(){
            if (this.instance && this.instance.Tally)
                if (this.instance.Tally.includes(this.$props.source) || typeof(this.$props.source)=='undefined')
                    return (this.instance.Streaming || this.instance.Recording);
                else
                    return false;
                // return this.instance.Tally.includes(this.$props.source) && (this.instance.Streaming || this.instance.Recording)
            else
                if (typeof(this.$props.source)=='undefined')
                    return (this.instance.Streaming || this.instance.Recording);
                else
                    return false;
        },

        previewMe(){
            if (this.instance && this.instance.PreviewTally)
                return this.instance.PreviewTally.includes(this.$props.source)
            else
                return false;
        },

        tally(){
            return (typeof(this.$props.source)!='undefined') ? this.$props.source : 'Program Out';
        }
    }
}
</script>

<style lang="stylus" scoped>

.tally {
    border: 10px transparent solid
    height: 99%;
    background: #333;
}

.message {
    color:silver;
    width:100%;
    top:37%;
    text-align:center;
    position fixed;
    font-size: 700%;
    opacity 30%
}

.label {
    
    color: white
    font-size 180%
    text-align center
    position fixed
    bottom .5em
    left .5em
    right .5em
    padding .1em

    .text{
        display inline-block
        border-radius: 2em;
        background-color: rgba(0,0,0,.4)
        padding 15px
    }
}

.live {
    border: 10px red solid;
    background red
}

.preview
{
    border: 10px red solid;
    animation: flash .7s infinite reverse ease;
}

.recording {
	background: rgba(255, 82, 82, 1);
	border-radius: 50%;
	margin: 10px;
	height: 20px;
	width: 20px;

	box-shadow: 0 0 0 0 rgba(255, 82, 82, 1);
	transform: scale(1);
	animation: pulse 2s infinite;
}

@keyframes flash {
  0% {
    border-color: transparent;
  }
  100% {
    border-color: #FF4136;
  }
}

@keyframes pulse {
	0% {
		transform: scale(0.95);
		box-shadow: 0 0 0 0 rgba(255, 82, 82, 0.7);
	}
	
	70% {
		transform: scale(1);
		box-shadow: 0 0 0 10px rgba(255, 82, 82, 0);
	}
	
	100% {
		transform: scale(0.95);
		box-shadow: 0 0 0 0 rgba(255, 82, 82, 0);
	}
}
</style>