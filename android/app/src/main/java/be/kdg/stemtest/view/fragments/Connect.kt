package be.kdg.stemtest.view.fragments

import android.content.Context
import android.opengl.Visibility
import androidx.lifecycle.ViewModelProviders
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.*
import androidx.core.view.isVisible
import androidx.lifecycle.LiveData
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProvider
import androidx.navigation.findNavController
import be.kdg.stemtest.R
import be.kdg.stemtest.model.entity.GameSettings
import be.kdg.stemtest.model.entity.Identification
import be.kdg.stemtest.viewmodel.ConnectViewModel
import dagger.android.AndroidInjector
import dagger.android.DispatchingAndroidInjector
import dagger.android.HasAndroidInjector
import dagger.android.support.AndroidSupportInjection
import life.sabujak.roundedbutton.RoundedButton
import pl.droidsonroids.gif.GifImageButton
import pl.droidsonroids.gif.GifImageView
import javax.inject.Inject


class Connect : Fragment(), HasAndroidInjector {


    private lateinit var viewModel: ConnectViewModel
    private lateinit var button: RoundedButton
    private lateinit var scanbtn: RoundedButton
    private lateinit var privacybtn: RoundedButton
    private lateinit var textView: TextView
    private lateinit var gif: GifImageView
    val code: Int = 0
    private lateinit var identificationData:LiveData<Identification>
    private lateinit var gameData:LiveData<GameSettings>

    @Inject
    lateinit var androidInjector: DispatchingAndroidInjector<Any>
    @Inject
    lateinit var viewModelFactory: ViewModelProvider.Factory

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.connect_fragment, container, false)
    }

    override fun onAttach(context: Context) {
        AndroidSupportInjection.inject(this)
        super.onAttach(context)
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        viewModel = ViewModelProviders.of(this, viewModelFactory)[ConnectViewModel::class.java]
        viewModel.deleteAll()

        initialiseViews(view)
        setEventHandlers()


    }

    private fun setEventHandlers() {
        button.setOnClickListener {
            getCode(view)
        }

        scanbtn.setOnClickListener { v ->
            val navController = v.findNavController()
            navController.navigate(R.id.action_connect_to_scanner)
        }

        privacybtn.setOnClickListener { v ->
            val navController = v.findNavController()
            navController.navigate(R.id.action_connect_to_privacyStatement)
        }
    }


    private fun getCode(view: View?) {
        val code = textView.text.toString().toIntOrNull()


        if (code != null) {
            identificationData= viewModel.makeStudentSession(code)

            gameData = viewModel.getGameType();
            val observer = Observer<GameSettings> { i ->
                    val navController = view?.findNavController()
                    when (i.gameType) {
                        1, 3, 4 -> {
                            navController?.navigate(R.id.action_connect_to_wachtscherm)
                        }
                        2 -> {
                            navController?.navigate(R.id.action_connect_to_selecteer_partij)
                        }
                    }
                }

            val identificationObserver= Observer<Identification>{i ->
                when(i.studentId){
                    -1 -> {
                        Toast.makeText(context,"Deze les zit vol!",Toast.LENGTH_LONG).show()
                        button.isClickable=true
                        scanbtn.isClickable=true
                    }

                    -2 -> {Toast.makeText(context,"De sessiecode kon niet worden gevonden!",Toast.LENGTH_LONG).show()
                        button.isClickable=true
                        scanbtn.isClickable=true

                    }
                    -3 -> {Toast.makeText(context,"Er kon geen connectie gemaakt worden",Toast.LENGTH_LONG).show()
                        button.isClickable=true
                        scanbtn.isClickable=true

                    }
                    else -> gameData.observe(viewLifecycleOwner, observer)
                }
            }

            //play animation
            gif.isVisible = true
            button.isClickable=false

            scanbtn.isClickable=false

            identificationData.observe(viewLifecycleOwner,identificationObserver)


            }

        }


    private fun initialiseViews(view: View) {
        textView = view.findViewById(R.id.etConnect)
        button = view.findViewById(R.id.btnConnect)
        scanbtn = view.findViewById(R.id.btnScanQr)
        privacybtn = view.findViewById(R.id.btnPrivacyConnect)
        gif = view.findViewById(R.id.gifConnect)
        val tv = view.findViewById<TextView>(R.id.etConnect)

        val code = arguments?.getInt("code", 0)!!
        if (code!=0){
            tv.text = code.toString()
            getCode(view)
        }


    }


    override fun androidInjector(): AndroidInjector<Any> {
        return androidInjector
    }

}

