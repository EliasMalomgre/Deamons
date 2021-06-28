package be.kdg.stemtest.view.fragments

import android.content.Context
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import android.widget.TextView
import android.widget.Toast
import androidx.activity.addCallback
import androidx.annotation.NonNull
import androidx.fragment.app.Fragment
import androidx.lifecycle.LiveData
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.ViewModelProviders
import androidx.navigation.findNavController
import be.kdg.stemtest.R
import be.kdg.stemtest.model.entity.Party
import be.kdg.stemtest.viewmodel.PgDgMoreInfoViewModel
import com.pierfrancescosoffritti.androidyoutubeplayer.core.player.YouTubePlayer
import com.pierfrancescosoffritti.androidyoutubeplayer.core.player.listeners.AbstractYouTubePlayerListener
import com.pierfrancescosoffritti.androidyoutubeplayer.core.player.utils.YouTubePlayerTracker
import com.pierfrancescosoffritti.androidyoutubeplayer.core.player.views.YouTubePlayerView
import com.squareup.picasso.Picasso
import dagger.android.AndroidInjector
import dagger.android.DispatchingAndroidInjector
import dagger.android.HasAndroidInjector
import dagger.android.support.AndroidSupportInjection
import kotlinx.android.synthetic.main.fragment_pgdg_more_info.view.*
import javax.inject.Inject


class MoreInfo : Fragment(), HasAndroidInjector {
    private lateinit var logo: ImageView
    private lateinit var video: YouTubePlayerView
    private lateinit var orientation: TextView
    private lateinit var leader: TextView
    private lateinit var colour: TextView

    private lateinit var party: Party
    private lateinit var partyName: String

    private lateinit var viewModel : PgDgMoreInfoViewModel

    private lateinit var partyData: LiveData<Party>

    private var gameType = 0;

    @Inject
    lateinit var androidInjector: DispatchingAndroidInjector<Any>
    @Inject
    lateinit var viewModelFactory: ViewModelProvider.Factory


    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        val view =inflater.inflate(R.layout.fragment_pgdg_more_info, container, false)
        if (arguments!=null){
            gameType = arguments?.getInt("gameType",0)!!
        }

        val callback = requireActivity().onBackPressedDispatcher.addCallback(this) {

            when (gameType) {
                1 -> view.findNavController().navigate(R.id.resultaat)
                2 -> view.findNavController().navigate(R.id.dgResult)
            }
        }

        return view
    }

    private fun initialiseVideo(youTubePlayerView: YouTubePlayerView, videoId: String){
        lifecycle.addObserver(youTubePlayerView)
        youTubePlayerView.addYouTubePlayerListener(object : AbstractYouTubePlayerListener() {
            override fun onReady(@NonNull youTubePlayer: YouTubePlayer) {
                youTubePlayer.loadVideo(videoId, 0f)
            }
        })
    }


    private fun initialiseViews(view:View){
        logo = view.ivMoreInfoLogo
        video = view.vvMoreInfo
        lifecycle.addObserver(video);
        orientation = view.tvMoreInfoOrien
        leader = view.tvMoreInfoLeader
        colour = view.tvMoreInfoColour
    }

    private fun loadElements(party: Party){
        Picasso.get().load(party.logo).into(logo)
        initialiseVideo(video,party.partyMediaLink!!)
        orientation.text = party.orientation
        leader.text = party.partyLeader
        colour.text = party.colour
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        viewModel = ViewModelProviders.of(this, viewModelFactory)[PgDgMoreInfoViewModel::class.java]
        initialiseViews(view)
        partyName = arguments?.get("partyName") as String

        partyData = viewModel.getParty(partyName)
        val partyObserver= Observer<Party>{i ->
            loadElements(i)
        }
        partyData.observe(viewLifecycleOwner,partyObserver)
    }


    override fun onAttach(context: Context) {
        AndroidSupportInjection.inject(this)
        super.onAttach(context)

    }

    override fun androidInjector(): AndroidInjector<Any> {
        return androidInjector
    }
}
