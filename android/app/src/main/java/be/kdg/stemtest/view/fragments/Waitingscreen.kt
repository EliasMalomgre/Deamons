package be.kdg.stemtest.view.fragments

import android.content.Context
import android.graphics.Color
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import android.widget.Toast
import androidx.activity.addCallback
import androidx.constraintlayout.widget.ConstraintLayout
import androidx.fragment.app.Fragment
import androidx.lifecycle.LiveData
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.ViewModelProviders
import androidx.navigation.findNavController
import be.kdg.stemtest.R
import be.kdg.stemtest.model.entity.GameSettings
import be.kdg.stemtest.model.entity.Identification
import be.kdg.stemtest.viewmodel.WachtschermViewModel
import com.microsoft.signalr.HubConnection
import dagger.android.AndroidInjector
import dagger.android.DispatchingAndroidInjector
import dagger.android.HasAndroidInjector
import dagger.android.support.AndroidSupportInjection
import io.reactivex.android.schedulers.AndroidSchedulers
import io.reactivex.schedulers.Schedulers
import pl.droidsonroids.gif.GifImageView
import javax.inject.Inject


class Waitingscreen : Fragment(),HasAndroidInjector {


    private lateinit var viewModel: WachtschermViewModel
    private lateinit var layout : ConstraintLayout
    private lateinit var connectionProblem : TextView
    private lateinit var gif : GifImageView

    private lateinit var identificationData:LiveData<Identification>
    private lateinit var gameSettingsData:LiveData<GameSettings>


    private  var colourIndex :Int = 0



    @Inject
    lateinit var androidInjector: DispatchingAndroidInjector<Any>
    @Inject
    lateinit var viewModelFactory: ViewModelProvider.Factory
    @Inject
     lateinit var hubConnection : HubConnection

    private var backPressed = false;


    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        val view = inflater.inflate(R.layout.wachtscherm_fragment, container, false);

        val callback = requireActivity().onBackPressedDispatcher.addCallback(this){
            if (!backPressed){
                Toast.makeText(context,"Weet je zeker dat je het spel wil verlaten?",Toast.LENGTH_LONG).show();
                backPressed = true;
            }
            else{
                view.findNavController().navigate(R.id.connect);
            }
        }
        return view;
    }


    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        viewModel = ViewModelProviders.of(this, viewModelFactory)[WachtschermViewModel::class.java]

        if (arguments!=null){
            colourIndex = arguments?.getInt("coloursArg",0)!!
        }

        initialiseViews(view)
        addEventHandlers(view)

        val currStatement= viewModel.getCurrStatement()
        var maxStatement=-1

        gameSettingsData=viewModel.getGameData()
        val gameSetingsObserver= Observer<GameSettings> { i ->
            hubConnection.on(
                "SessionPosition", { position, amountOfStatements ->
                    maxStatement = amountOfStatements.toInt()
                    if (currStatement == amountOfStatements.toInt() && position > amountOfStatements) {
                        hubConnection.stop()
                            .subscribeOn(Schedulers.io())
                            .observeOn(AndroidSchedulers.mainThread())
                            .doOnError { connectionProblem.visibility=View.VISIBLE }
                            .retry()
                            .doOnComplete {
                                val navController = view?.findNavController()

                                when(i.gameType){
                                    1 ->  navController.navigate(R.id.action_wachtscherm_to_dgResult)
                                    2 -> navController.navigate(R.id.action_wachtscherm_to_resultaat2)
                                    3 -> navController.navigate(R.id.action_wachtscherm_to_CPgResult)
                                    4 -> navController.navigate(R.id.action_wachtscherm_to_CDgResult)
                                }

                            }
                            .subscribe()

                    } else if (position.toInt() > -1 && viewModel.getCurrStatement() <= position.toInt()) {
                        hubConnection.stop()
                            .subscribeOn(Schedulers.io())
                            .observeOn(AndroidSchedulers.mainThread())
                            .doOnComplete {
                                val navController = view?.findNavController()
                                navController.navigate(R.id.action_wachtscherm_to_dgStelling)
                            }
                            .subscribe()
                    }
                },
                String::class.java, String::class.java
            )



            hubConnection.on("StopWaiting", { tsPosition ->
                if (maxStatement != -1 && tsPosition.toInt() + 1 == maxStatement) {
                    hubConnection.stop()
                        .subscribeOn(Schedulers.io())
                        .observeOn(AndroidSchedulers.mainThread())
                        .doOnComplete {
                            val navController = view?.findNavController()
                            when(i.gameType){
                                1 ->  navController.navigate(R.id.action_wachtscherm_to_dgResult)
                                2 -> navController.navigate(R.id.action_wachtscherm_to_resultaat2)
                                3 -> navController.navigate(R.id.action_wachtscherm_to_CPgResult)
                                4 -> navController.navigate(R.id.action_wachtscherm_to_CDgResult)
                            }
                        }
                        .doOnError { connectionProblem.visibility=View.VISIBLE }
                        .retry()
                        .subscribe()
                } else if (currStatement <= tsPosition.toInt() + 1) {
                    hubConnection.stop()
                        .subscribeOn(Schedulers.io())
                        .observeOn(AndroidSchedulers.mainThread())
                        .doOnComplete {
                            val navController = view.findNavController()
                            navController.navigate(R.id.action_wachtscherm_to_dgStelling)
                        }
                        .subscribe()

                }
            }, String::class.java)


            if (i.gameType==1 || i.gameType==4) {

                when (colourIndex) {
                    1 -> layout.setBackgroundColor(Color.parseColor(i.colour1))
                    2 -> layout.setBackgroundColor(Color.parseColor(i.colour2))
                    3 -> layout.setBackgroundColor(Color.parseColor(i.colourSkip))
                    4 -> layout.setBackgroundColor(Color.parseColor(i.colour3))
                    5 -> layout.setBackgroundColor(Color.parseColor(i.colour4))
                    6 -> layout.setBackgroundColor(Color.parseColor(i.colour5))
                    7 -> layout.setBackgroundColor(Color.parseColor(i.colour6))
                }
            }




        }



        gameSettingsData.observe(viewLifecycleOwner,gameSetingsObserver)
        identificationData=viewModel.getIdentification()
        val observer = Observer<Identification>{i ->
            if (i !=null) {
                hubConnection.start()
                    .subscribeOn(Schedulers.io())
                .observeOn(AndroidSchedulers.mainThread())
                    .doOnError { connectionProblem.visibility=View.VISIBLE }
                    .retry()
                    .doOnComplete {
                        hubConnection.invoke(Void::class.java,"AddToGroup", i.sessionCode.toString())
                        hubConnection.invoke(Void::class.java,"GetTeacherPosition", i.sessionCode.toString())
                        hubConnection.invoke(Void::class.java,"RefreshTeacherResults", i.sessionCode.toString())
                    }
                    .subscribe()
            }
        }
        identificationData.observe(viewLifecycleOwner,observer)


    }




    private fun addEventHandlers(view: View) {
        view.findViewById<TextView>(R.id.txtQuiznaam).setOnClickListener{
                v ->
            val navController = v.findNavController()
            navController.navigate(R.id.action_wachtscherm_to_dgStelling)
        }

    }

    private fun initialiseViews(view: View) {
        layout=view.findViewById(R.id.waitingscreenLayout)
        connectionProblem = view.findViewById(R.id.connectionProblem)
        gif = view.findViewById(R.id.gifConnect)

    }

    override fun onAttach(context: Context) {
        AndroidSupportInjection.inject(this)
        super.onAttach(context)
    }

    override fun androidInjector(): AndroidInjector<Any> {
        return androidInjector
    }


}
