package be.kdg.stemtest.view.fragments.partygame

import android.content.Context
import androidx.lifecycle.ViewModelProviders
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import android.widget.Toast
import androidx.activity.addCallback
import androidx.core.os.bundleOf
import androidx.lifecycle.LiveData
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProvider
import androidx.navigation.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import be.kdg.stemtest.R
import be.kdg.stemtest.model.entity.*
import be.kdg.stemtest.model.entity.Statement
import be.kdg.stemtest.viewmodel.ResultaatViewModel
import dagger.android.AndroidInjector
import dagger.android.DispatchingAndroidInjector
import dagger.android.HasAndroidInjector
import dagger.android.support.AndroidSupportInjection
import kotlinx.android.synthetic.main.resultaat_fragment.*
import life.sabujak.roundedbutton.RoundedButton
import javax.inject.Inject


class PgResultaat : Fragment(),HasAndroidInjector {



   private lateinit var partyName: TextView
    private lateinit var score:TextView
    private lateinit var infoBtn:RoundedButton
    private lateinit var recyclerView: RecyclerView
    private lateinit var viewAdapter: PgResultAdapter
    private lateinit var viewManager: LinearLayoutManager

    private lateinit var viewModel:ResultaatViewModel

    private lateinit var resultData:LiveData<String>
    private lateinit var partyData:LiveData<Party>
    private lateinit var answerOptionData:LiveData<List<AnswerOption>>
    private lateinit var answerData:LiveData<List<StudentAnswer>>
    private lateinit var partyAnswerData:LiveData<List<PartyAnswer>>
    private lateinit var statementData:LiveData<List<Statement>>
    private var errorShown=false

    private lateinit var party: Party


    @Inject
    lateinit var androidInjector: DispatchingAndroidInjector<Any>

    @Inject
    lateinit var viewModelFactory: ViewModelProvider.Factory

    private var backPressed = false;


    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        val view = inflater.inflate(R.layout.resultaat_fragment, container, false)
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
        viewModel = ViewModelProviders.of(this, viewModelFactory)[ResultaatViewModel::class.java]
        initialiseViews(view)
        addEventHandlers(view)
        getData()
        fillViews(view)
    }


    private fun getData() {
        resultData = viewModel.getResult()
        val resultObserver = androidx.lifecycle.Observer<String>{s->
            if (s.equals("error")){
                if (errorShown==false){
                    errorShown=true
                    Toast.makeText(context, "Kon het resultaat niet ophalen",Toast.LENGTH_SHORT).show()
                }
            }else{
                errorShown=false
                score.text = s
            }
        }

        resultData.observe(viewLifecycleOwner,resultObserver)

        partyData = viewModel.getParty()
        val partyObserver = Observer<Party>{
                p -> chosenParty.text = p.name
            party = p
        }
        partyData.observe(viewLifecycleOwner,partyObserver)

        answerOptionData=viewModel.getAnswerOptions()
        val antwoordMogelijkheidObserver= Observer<List<AnswerOption>>{
                s -> viewAdapter.setAnswerOptions(s)
        }
        answerOptionData.observe(viewLifecycleOwner,antwoordMogelijkheidObserver)

        answerData = viewModel.getAnswers()
        val answerObserver= Observer<List<StudentAnswer>>{
                s -> viewAdapter.setStudentAnswers(s)
        }
        answerData.observe(viewLifecycleOwner,answerObserver)

        statementData = viewModel.getStatements()
        val stellingObserver = Observer<List<Statement>>{
                s -> viewAdapter.setStelling(s)
        }
        statementData.observe(viewLifecycleOwner,stellingObserver)

        partyAnswerData = viewModel.getPartyAnswers()
        val partyAnswerObserver= Observer<List<PartyAnswer>>{
                s -> viewAdapter.setPartyAnswer(s)
            recyclerView.adapter = viewAdapter
        }
        partyAnswerData.observe(viewLifecycleOwner,partyAnswerObserver)
    }

    private fun fillViews(view: View) {
        recyclerView.apply {
            setHasFixedSize(true)
            layoutManager=viewManager
            adapter=viewAdapter
        }
    }

    private fun addEventHandlers(view:View) {
        infoBtn.setOnClickListener{loadMoreInfo(party)}
    }

    override fun onAttach(context: Context) {
        AndroidSupportInjection.inject(this)
        super.onAttach(context)
    }

    private fun loadMoreInfo(party: Party){
        val navController = view?.findNavController()
        navController?.navigate(R.id.action_resultaat_to_pgdg_more_info,  bundleOf("partyName" to party.name,"gameType" to 1))
    }


    private fun initialiseViews(view: View) {
        partyName = view.findViewById(R.id.chosenParty)
        score = view.findViewById(R.id.score)
        infoBtn = view.findViewById(R.id.btnMoreInfo)
        recyclerView = view.findViewById(R.id.ResultaatRv)
        viewManager = LinearLayoutManager(context)
        viewAdapter =
            PgResultAdapter(context)
    }

    override fun androidInjector(): AndroidInjector<Any> {
        return androidInjector
    }
}
