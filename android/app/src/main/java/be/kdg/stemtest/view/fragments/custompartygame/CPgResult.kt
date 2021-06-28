package be.kdg.stemtest.view.fragments.custompartygame

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
import androidx.lifecycle.LiveData
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProvider
import androidx.navigation.findNavController
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import be.kdg.stemtest.R
import be.kdg.stemtest.model.entity.*
import be.kdg.stemtest.viewmodel.CPgResultViewModel
import dagger.android.AndroidInjector
import dagger.android.DispatchingAndroidInjector
import dagger.android.HasAndroidInjector
import dagger.android.support.AndroidSupportInjection
import javax.inject.Inject


class CPgResult : Fragment(),HasAndroidInjector {

    private lateinit var resultData: LiveData<String>
    private lateinit var answerOptionData: LiveData<List<AnswerOption>>
    private lateinit var answerData: LiveData<List<StudentAnswer>>
    private lateinit var correctAnswerData: LiveData<List<AnswerOption>>
    private lateinit var statementData: LiveData<List<Statement>>
    private var errorShown = false


    @Inject
    lateinit var androidInjector: DispatchingAndroidInjector<Any>

    @Inject
    lateinit var viewModelFactory: ViewModelProvider.Factory

    private lateinit var score: TextView
    private lateinit var recyclerView: RecyclerView
    private lateinit var viewAdapter: CPgResultAdapter
    private lateinit var viewManager: LinearLayoutManager

    private lateinit var viewModel: CPgResultViewModel

    private var backPressed = false;



    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        val view = inflater.inflate(R.layout.c_pg_result_fragment, container, false)
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
        viewModel = ViewModelProviders.of(this, viewModelFactory)[CPgResultViewModel::class.java]
        initialiseViews(view)
        getData()
        fillViews(view)
    }

    private fun getData() {


        resultData = viewModel.getResult()
        val resultObserver = androidx.lifecycle.Observer<String>{
                s ->
            if (s.equals("error")){
                if (errorShown==false){
                    Toast.makeText(context,"Kon geen resultaten ophalen",Toast.LENGTH_LONG).show()
                    errorShown=true
                }
            }else{
                errorShown=false
                score.text = s
            }
        }

        resultData.observe(viewLifecycleOwner,resultObserver)


        answerOptionData=viewModel.getAnswerOptions()
        val answerOptionObserver= Observer<List<AnswerOption>>{
                s -> viewAdapter.setAnswerOptions(s)
        }
        answerOptionData.observe(viewLifecycleOwner,answerOptionObserver)

        answerData = viewModel.getAnswers()
        val answerObserver= Observer<List<StudentAnswer>>{
                s -> viewAdapter.setStudentAnswers(s)
        }
        answerData.observe(viewLifecycleOwner,answerObserver)

        statementData = viewModel.getStatements()
        val statementObserver = Observer<List<Statement>>{
                s -> viewAdapter.setStatement(s)
        }
        statementData.observe(viewLifecycleOwner,statementObserver)

        correctAnswerData = viewModel.getCorrectAnswers()
        val correctAnswerObserver= Observer<List<AnswerOption>> { s ->
            if (!s.isEmpty()) {
                if (s.first().id == -1) {
                    if (errorShown == false) {
                        Toast.makeText(context, "Kon geen antwoorden ophalen", Toast.LENGTH_LONG)
                            .show()
                        errorShown = true
                    }
                } else {
                    errorShown = false
                    viewAdapter.setCorrectAnswer(s)
                    recyclerView.adapter = viewAdapter
                }
            }
        }
        correctAnswerData.observe(viewLifecycleOwner,correctAnswerObserver)
    }

    private fun initialiseViews(view: View) {
        score = view.findViewById(R.id.scoreCPg)
        recyclerView = view.findViewById(R.id.rv_CPg)
        viewManager = LinearLayoutManager(context)
        viewAdapter =
            CPgResultAdapter(context)
    }

    private fun fillViews(view: View) {
        recyclerView.apply {
            setHasFixedSize(true)
            layoutManager=viewManager
            adapter=viewAdapter
        }
    }

    override fun androidInjector(): AndroidInjector<Any> {
      return androidInjector
    }

    override fun onAttach(context: Context) {
        AndroidSupportInjection.inject(this)
        super.onAttach(context)
    }
}
