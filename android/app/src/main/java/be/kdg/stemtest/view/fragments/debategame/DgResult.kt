package be.kdg.stemtest.view.fragments.debategame

import android.content.Context
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.*
import androidx.activity.addCallback
import androidx.core.os.bundleOf
import androidx.fragment.app.Fragment
import androidx.lifecycle.LiveData
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.ViewModelProviders
import androidx.navigation.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import be.kdg.stemtest.R
import be.kdg.stemtest.model.entity.*
import be.kdg.stemtest.viewmodel.DgResultViewModel
import com.google.android.material.bottomnavigation.BottomNavigationView
import com.microsoft.signalr.HubConnection
import dagger.android.AndroidInjector
import dagger.android.DispatchingAndroidInjector
import dagger.android.HasAndroidInjector
import dagger.android.support.AndroidSupportInjection
import io.reactivex.android.schedulers.AndroidSchedulers
import io.reactivex.schedulers.Schedulers
import javax.inject.Inject


class DgResult : Fragment(), HasAndroidInjector {



    private lateinit var recyclerView: RecyclerView
    private lateinit var viewAdapter: DgResultStatementAdapter
    private lateinit var viewManager: LinearLayoutManager
    private lateinit var viewModel: DgResultViewModel

    private lateinit var resultData: LiveData<List<DebateGameResult>>
    private lateinit var partyData: LiveData<List<Party>>
    private lateinit var partyAnswerData: LiveData<List<PartyAnswer>>
    private lateinit var answerData: LiveData<List<StudentAnswer>>
    private lateinit var statementData: LiveData<List<Statement>>
    private lateinit var answerOptionsData: LiveData<List<AnswerOption>>
    private lateinit var identificationData: LiveData<Identification>

    private lateinit var botnav: BottomNavigationView

    private lateinit var statements: List<Statement>
    private lateinit var parties:List<Party>
    private lateinit var partyAnswers:List<PartyAnswer>
    private lateinit var answers:List<StudentAnswer>
    private lateinit var answerOptions:List<AnswerOption>

    private var errorShown = false

    private var backPressed = false;



    @Inject
    lateinit var androidInjector: DispatchingAndroidInjector<Any>

    @Inject
    lateinit var viewModelFactory: ViewModelProvider.Factory

    @Inject
    lateinit var hubConnection : HubConnection


    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        val view = inflater.inflate(R.layout.dg_result_fragment, container, false)
        val callback = requireActivity().onBackPressedDispatcher.addCallback(this) {
            if (!backPressed) {
                Toast.makeText(
                    context,
                    "Weet je zeker dat je het spel wil verlaten?",
                    Toast.LENGTH_LONG
                ).show();
                backPressed = true;
            } else {
                view.findNavController().navigate(R.id.connect);
            }
        }
        return view;
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        viewModel = ViewModelProviders.of(this, viewModelFactory)[DgResultViewModel::class.java]
        initialiseViews(view)
        addEventHandlers(view)


        partyData = viewModel.getParties()
        val partyObserver = Observer<List<Party>> { i ->
            viewAdapter.setPartytData(i)
            recyclerView.adapter = viewAdapter
            parties = i

            //spinner.adapter= ArrayAdapter<String>(requireContext(),R.layout.support_simple_spinner_dropdown_item,i.map { i -> i.name }.toTypedArray())
        }

        partyData.observe(viewLifecycleOwner, partyObserver)


        resultData = viewModel.getResult()
        val resultObserver = Observer<List<DebateGameResult>> { i ->
            if (!i.isEmpty()) {
                if (i.first().partyName.equals("error")) {
                    if (errorShown == false) {
                        Toast.makeText(context, "Kon geen resultaten ophalen", Toast.LENGTH_LONG)
                            .show()
                        errorShown = true
                    }
                }
                errorShown = false
                viewAdapter.setResultData(i)
                recyclerView.adapter = viewAdapter
            }
        }
        resultData.observe(viewLifecycleOwner, resultObserver)

        partyAnswerData = viewModel.getPartyAnswers()
        val partyAnswerObserver = Observer<List<PartyAnswer>> {i ->

            partyAnswers=i
        }
        partyAnswerData.observe(viewLifecycleOwner,partyAnswerObserver)

        answerData = viewModel.getAnswers()
        val answerObserver = Observer<List<StudentAnswer>> { i ->
            answers=i
        }
        answerData.observe(viewLifecycleOwner,answerObserver)

        statementData = viewModel.getStatements()
        val statementObserver= Observer<List<Statement>> { i ->
            statements = i
        }
        statementData.observe(viewLifecycleOwner,statementObserver)


        answerOptionsData = viewModel.getAnswerOptions()
        val answerOptionsObserver= Observer<List<AnswerOption>> { i ->
            answerOptions = i
        }
        answerOptionsData.observe(viewLifecycleOwner,answerOptionsObserver)

        identificationData= viewModel.getIdentification()
        val identificationObserver = Observer<Identification> { i ->
            hubConnection.start()
                .subscribeOn(Schedulers.io())
                .observeOn(AndroidSchedulers.mainThread())
                .doOnError {
                    Toast.makeText(
                        context,
                        "SignalR kon niet gestart worden",
                        Toast.LENGTH_SHORT
                    ).show()
                }
                .doOnComplete {
                    hubConnection.invoke(Void::class.java,"RefreshTeacherResults", i.sessionCode.toString())
                }
                .subscribe()
        }
        identificationData.observe(viewLifecycleOwner,identificationObserver)
        fillViews(view)
    }

    private fun fillViews(view: View) {
        recyclerView.apply {
            setHasFixedSize(true)
            layoutManager = viewManager
            adapter = viewAdapter
        }
    }

    private fun addEventHandlers(view: View) {
        botnav.setOnNavigationItemSelectedListener(navListener)
    }

    override fun onAttach(context: Context) {
        AndroidSupportInjection.inject(this)
        super.onAttach(context)
    }


    private fun initialiseViews(view: View) {
        viewManager = LinearLayoutManager(context)
        viewAdapter =
            DgResultStatementAdapter(context)
        botnav = view.findViewById(R.id.dgBotNav)
        recyclerView = view.findViewById(R.id.rvDgResult)
    }

    override fun androidInjector(): AndroidInjector<Any> {
        return androidInjector
    }



    private fun replaceFragment(fragment: Fragment){
        val fragmentTransaction=childFragmentManager.beginTransaction()
        fragment.arguments = bundleOf("parties" to parties, "partyArg" to partyAnswers, "answerArg" to answers,"statementArg" to statements,"answerOptionsArg" to answerOptions)
        fragmentTransaction.replace(R.id.dgResultContainer, fragment)
        fragmentTransaction.commit()
    }


    private val navListener = BottomNavigationView.OnNavigationItemSelectedListener { item ->
        when (item.itemId) {
            R.id.page_1 -> {
                println("1")
                replaceFragment(DgOverview())

                return@OnNavigationItemSelectedListener true
            }
            R.id.page_2 -> {
                println("2")
                replaceFragment(PartijInfo())
                return@OnNavigationItemSelectedListener true
            }
            else -> return@OnNavigationItemSelectedListener false
        }
    }



}


